{{/*
Chart name (overridable via nameOverride).
*/}}
{{- define "inventory.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Full resource name. Defaults to the Helm release name (e.g. "inventory").
*/}}
{{- define "inventory.fullname" -}}
{{- default .Release.Name .Values.fullnameOverride | trunc 63 | trimSuffix "-" -}}
{{- end -}}

{{/*
Common labels stamped on every resource.
*/}}
{{- define "inventory.labels" -}}
app.kubernetes.io/name: {{ include "inventory.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
helm.sh/chart: {{ .Chart.Name }}-{{ .Chart.Version }}
{{- end -}}

{{/*
Selector labels — the stable subset used to match pods to Deployments/Services.
Must NOT include version/chart labels (those change and would break selectors).
*/}}
{{- define "inventory.selectorLabels" -}}
app.kubernetes.io/name: {{ include "inventory.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end -}}

{{/*
DB connection env vars, sourced from the CNPG-generated secret and composed into
the connection string the app expects. Shared by the Deployment and the migration
Job so the two can never drift. Include with `nindent 12` under an `env:` key.
*/}}
{{- define "inventory.dbEnv" -}}
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
- name: ConnectionStrings__Inventory
  value: "Host=$(DB_HOST);Port=$(DB_PORT);Database=$(DB_NAME);Username=$(DB_USER);Password=$(DB_PASSWORD)"
{{- end -}}
