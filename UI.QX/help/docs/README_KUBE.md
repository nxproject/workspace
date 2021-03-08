# NX.Node - Kubernetes

While NX.Node has its own method for inter-bee connectivity and hive health checks,
it also provides lite-weight support for Kubeernetes livelinesss and readiness probes.

This is entry in the deployment for the liveliness probe:
```
 livenessProbe:
  # an http probe
  httpGet:
    path: /liveliness
    port: 80
  initialDelaySeconds: 15
  timeoutSeconds: 1
```
and for the readiness probe:
```
  readinessProbe:
    # an http probe
    httpGet:
      path: /readiness
      port: 8080
    initialDelaySeconds: 20
    timeoutSeconds: 5
```

The is extra code that needs to be setup for the readiness probe, as each bee's requirement
will differ.  This is the code to support the probe:
```JavaScript
env.ReadinessCallback = delegate()
{
  ...

  return "ERROR MESSAGE or NULL IF OK";
}

[Back to top](/help/docs/README.md)
