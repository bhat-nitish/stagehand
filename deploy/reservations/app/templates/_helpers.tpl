{{/*
Chart name (overridable via nameOverride).
*/}}
{{- define "reservations.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Full resource name. Defaults to the Helm release name (e.g. "reservations").
*/}}
{{- define "reservations.fullname" -}}
{{- default .Release.Name .Values.fullnameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Common labels stamped on every resource.
*/}}
{{- define "reservations.labels" -}}
app.kubernetes.io/name: {{ include "reservations.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
helm.sh/chart: {{ .Chart.Name }}-{{ .Chart.Version }}
{{- end -}}

{{/*
Selector labels — the stable subset used to match pods to Deployments/Services.
Must NOT include version/chart labels (those change and would break selectors).
*/}}
{{- define "reservations.selectorLabels" -}}
app.kubernetes.io/name: {{ include "reservations.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end -}}

{{/*
DB connection env vars, sourced from the CNPG-generated secret and composed into
the connection string the app expects. Shared by the Deployment and the migration
Job so the two can never drift. Include with `nindent 12` under an `env:` key.
*/}}
{{- define "reservations.dbEnv" -}}
- name: DB_HOST
  valueFrom:
    secretKeyRef:
      name: {{ .Values.database.secretName }}
      key: host
- name: DB_PORT
  valueFrom:
    secretKeyRef:
      name: {{ .Values.database.secretName }}
      key: port
- name: DB_NAME
  valueFrom:
    secretKeyRef:
      name: {{ .Values.database.secretName }}
      key: dbname
- name: DB_USER
  valueFrom:
    secretKeyRef:
      name: {{ .Values.database.secretName }}
      key: username
- name: DB_PASSWORD
  valueFrom:
    secretKeyRef:
      name: {{ .Values.database.secretName }}
      key: password
- name: ConnectionStrings__Reservations
  value: "Host=$(DB_HOST);Port=$(DB_PORT);Database=$(DB_NAME);Username=$(DB_USER);Password=$(DB_PASSWORD)"
{{- end -}}

{{/*
RabbitMQ connection string. Required by the migration Job as well as the
Deployment: Program.cs registers messaging during service registration, before
the "migrate" branch runs, so a missing connection string fails the Job.
*/}}
{{- define "reservations.rabbitEnv" -}}
- name: RABBIT_USER
  valueFrom:
    secretKeyRef:
      name: {{ .Values.rabbitmq.secretName }}
      key: username
- name: RABBIT_PASSWORD
  valueFrom:
    secretKeyRef:
      name: {{ .Values.rabbitmq.secretName }}
      key: password
- name: ConnectionStrings__rabbitmq
  value: "amqp://$(RABBIT_USER):$(RABBIT_PASSWORD)@{{ .Values.rabbitmq.host }}:{{ .Values.rabbitmq.port }}"
{{- end -}}
