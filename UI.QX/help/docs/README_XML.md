# NX.Node - HTTP

While the default POST data and returns are JSON, the system can easily work with XML
data instead.  This is done by the use of the **response_format** environment setting.

Value|Meaning
-----|-------
auto|Response format mirrors the POST data format.  This is the default behavior
json|Force all responses to use JSON
xml|Force all responses to use XML

Note that the POST body is always treated as **auto**.

#### XML formatting rules

There is a restriction on how XML is formatted, it must always have a single tag labeled
**data** as the root.  For example:
```XML
<root>
  <key>size</key>
  <value>large</size>
</root>
```
is the equivalent of:
```JavaScript
{
  "key": "size",
  "value": "large"
}
```
Note that some of the JSON used in the system may have keys that are prefixed with
the **$$** prefix, which is not a valid XML tag.

[Back to top](/help/docs/README.md)
