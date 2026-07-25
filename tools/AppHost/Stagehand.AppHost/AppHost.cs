var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("stagehand-postgres-data")
    .WithLifetime(ContainerLifetime.Persistent);

var keycloak = builder.AddContainer("keycloak", "quay.io/keycloak/keycloak", "26.1")
.WithHttpEndpoint(port: 8081, targetPort: 8080, name: "http")
.WithArgs("start-dev", "--import-realm")
.WithEnvironment("KC_BOOTSTRAP_ADMIN_USERNAME", "admin")
.WithEnvironment("KC_BOOTSTRAP_ADMIN_PASSWORD", "admin")
.WithBindMount("../../../deploy/platform/keycloak/base", "/opt/keycloak/data/import");

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin(port: 15672)
    .WithDataVolume("stagehand-rabbitmq-data")
    .WithLifetime(ContainerLifetime.Persistent);

var catalogDb = postgres.AddDatabase("catalog");
var inventoryDb = postgres.AddDatabase("inventory");
var reservationsDb = postgres.AddDatabase("reservations");

var catalogApi = builder.AddProject<Projects.Stagehand_Catalog_Api>("catalog-api")
    .WithReference(catalogDb)
    .WaitFor(catalogDb)
    .WaitFor(keycloak);

var inventoryApi = builder.AddProject<Projects.Stagehand_Inventory_Api>("inventory-api")
    .WithReference(inventoryDb)
    .WithReference(rabbitmq)
    .WaitFor(inventoryDb)
    .WaitFor(rabbitmq)
    .WaitFor(keycloak);

var reservationsApi = builder.AddProject<Projects.Stagehand_Reservations_Api>("reservations-api")
    .WithReference(reservationsDb)
    .WithReference(rabbitmq)
    .WaitFor(reservationsDb)
    .WaitFor(rabbitmq)
    .WaitFor(keycloak);

builder.AddProject<Projects.Stagehand_Gateway>("gateway")
.WithEnvironment(
    "ReverseProxy__Clusters__catalog-cluster__Destinations__catalog__Address",
    catalogApi.GetEndpoint("http"))
.WithEnvironment(
    "ReverseProxy__Clusters__inventory-cluster__Destinations__inventory__Address",
    inventoryApi.GetEndpoint("http"))
.WithEnvironment(
    "ReverseProxy__Clusters__reservations-cluster__Destinations__reservations__Address",
    reservationsApi.GetEndpoint("http"))
.WaitFor(catalogApi)
.WaitFor(inventoryApi)
.WaitFor(reservationsApi);

builder.Build().Run();
