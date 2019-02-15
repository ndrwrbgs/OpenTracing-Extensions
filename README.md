[![NuGet](https://img.shields.io/nuget/v/OpenTracing.Contrib.Extensions.svg)](https://www.nuget.org/packages/OpenTracing.Contrib.Extensions)

# OpenTracing-Extensions
Simple extensions on top of the OpenTracing APIs.

Notable pieces...

## ISpanBuilder.WithTagsFromAnonymousType
_This name is verbose, don't be surprised if it changes slightly :)_

CAVEAT!!! Only valid to use if the user is NOT dynamically creating an unending multitude of dynamic runtime types - since we are caching. You shouldn't be doing this, I've never seen it done, but please be cognizant of this when calling this method.

Allows setting tags from an anonymous type. E.g. setting arguments from a method easily.

### Before and After
```diff
void MyMethod(string a, int b, ulong c, bool d)
{
  using (GlobalTracer.Instance
    .BuildSpan("operationName")
-   .WithTag(nameof(a), a)
-   .WithTag(nameof(b), b)
-   .WithTag(nameof(c), c)
-   .WithTag(nameof(d), d)
+   // Automatically captures the parameter names due to how anonymous types work :-)
+   .WithTagsFromAnonymousType(new { a, b, c, d })
    .StartActive())
  {
  }
}
```

## ISpan.LogMemberInfo
_And its related `ISpan.LogWithMemberInfo` and `ISpanBuilder.WithCallerMemberNameTag`_

_I'm not sure LogMemberInfo is actually useful aside from the other two methods, it might be removed_

Log the member name (and even line number) easily.

### Before and After
```diff
void MyMethod()
{
  using (var scope = GlobalTracer.Instance
    .BuildSpan("operationName")
-    // Hopefully doesn't get refactored to another method :-S
-    .SetTag("fromMethod", nameof(MyMethod))
+    .WithCallerMemberNameTag(tagName: "fromMethod")
    .StartActive())
  {
    scope.Span
-      .Log(
+      .LogWithMemberInfo(
        // [nit] - Dictionary is not as efficient as an Array of KeyValuePair's, but it is a nicer syntax :)
        fields: new Dictionary<string, object>(3)
        {
-         ["memberName"] = nameof(MyMethod),
-         ["memberLineNumber"] = /* This actually isn't possible */,
          ["someKey"] = true
        });
  }
}
```

## ISpan.Log(string key, object value)
_And its related overloads_

Allows logging a single `KeyValuePair` to the `ISpan`, without having the noise of initializing an array in your log code.

### Before and After
```diff
ISpan span;
-span.Log(
-  new []
-  {
-    new KeyValuePair<string, object>("myField", "myValue"),
-  });
+span.Log("myField", "myValue");
```
