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

var catalogDb = postgres.AddDatabase("catalog");

var catalogApi = builder.AddProject<Projects.Stagehand_Catalog_Api>("catalog-api")
    .WithReference(catalogDb)
    .WaitFor(catalogDb)
    .WaitFor(keycloak);

builder.AddProject<Projects.Stagehand_Gateway>("gateway")
.WithEnvironment(
    "ReverseProxy__Clusters__catalog-cluster__Destinations__catalog__Address",
    catalogApi.GetEndpoint("http"))
.WaitFor(catalogApi);

builder.Build().Run();
