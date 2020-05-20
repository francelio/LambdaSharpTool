---
title: LambdaSharp "Hicetas" Release (v0.8)
description: Release notes for LambdaSharp "Hicetas" (v0.8)
keywords: release, notes, hicetas
---

# LambdaSharp "Hicetas" Release (v0.8.0.2) - TBD

> Hicetas was a Greek philosopher of the Pythagorean School. He was born in Syracuse. Like his fellow Pythagorean Ecphantus and the Academic Heraclides Ponticus, he believed that the daily movement of permanent stars was caused by the rotation of the Earth around its axis. When Copernicus referred to Nicetus Syracusanus (Nicetus of Syracuse) in _De revolutionibus orbium coelestium_ as having been cited by Cicero as an ancient who also argued that the Earth moved, it is believed that he was actually referring to Hicetas. [(Wikipedia)](https://en.wikipedia.org/wiki/Hicetas)


## What's New

This release introduces some key new capabilities for Lambda functions and the _LambdaSharp.Core_ module. The [ALambdaFunction](xref:LambdaSharp.ALambdaFunction) base class has new methods for [sending events](xref:LambdaSharp.ALambdaFunction.SendEvent``1(System.String,System.String,``0,System.Collections.Generic.IEnumerable{System.String})), [capturing metrics](xref:LambdaSharp.ALambdaFunction.LogMetric(System.Collections.Generic.IEnumerable{LambdaSharp.LambdaMetric})), and [logging debug statements](xref:LambdaSharp.ALambdaFunction.LogDebug(System.String,System.Object[])). In addition, the _LambdaSharp.Core_ module now uses [Amazon Kinesis Firehose](https://aws.amazon.com/kinesis/data-firehose/) for ingesting CloudWatch Log streams. Kinesis Firehose is more cost effective, scales automatically, and also provides built-in integration with S3 for retaining ingested logs. Finally, during the ingestion process, the logs are converted into [queryable JSON documents](LogRecords.md).

### Upgrading from v0.7 to v0.8
1. Ensure all modules are deployed with v0.6.1 or later
1. Upgrade LambdaSharp CLI to v0.8
    1. `dotnet tool update -g LambdaSharp.Tool`
1. Upgrade LambdaSharp Deployment Tier (replace `Sandbox` with the name of the deployment tier to upgrade)
    1. `lash init --allow-upgrade --tier Sandbox`


## BREAKING CHANGES

### LambdaSharp Core Services

#### LambdaSharp.Core

Some configuration parameters changed with the switch to [Amazon Kinesis Firehose](https://aws.amazon.com/kinesis/data-firehose/) for CloudWatch logs ingestion.
* The `LoggingStreamRetentionPeriodHours` and `LoggingStreamShardCount` are no longer needed and have been removed.
* The `LoggingStream` has been replaced by `LoggingFirehoseStream`.
* The `LoggingBucketSuccessPrefix` and `LoggingBucketFailurePrefix` have been added.

See the [LambdaSharp.Core](~/modules/LambdaSharp-Core.md) module documentation for more details.


### LambdaSharp SDK

**IMPORTANT:** In preparation to switching `LambdaSharp` assembly to .NET Core 3.1 and using [System.Text.Json](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to) as JSON serializer in a future release, it is highly advised to remove all references to `Amazon.Lambda.Serialization.Json` and `Newtonsoft.Json` assemblies from all _.csproj_ files. These package are instead inherited via the `LambdaSharp` assembly. These changes ensure that once `LambdaSharp` assembly switches its serialization implementation, dependent projects will cease to compile, making it easier to transition to the new JSON serializer. Otherwise, these projects would compile successfully, but fail at runtime, as it is not possible to mix `Newtonsoft.Json` and `System.Text.Json` serializations.

Look for the following package references in your _.csproj_ files and remove them.
```xml
<PackageReference Include="Amazon.Lambda.Core" Version="1.1.0" />
<PackageReference Include="Amazon.Lambda.Serialization.Json" Version="1.5.0" />
<PackageReference Include="Newtonsoft.Json" Version="12.0.3" />
```

In addition, Lambda functions no longer need to declare the default serializer with the `[assembly: LambdaSerializer(...)]` attribute, unless a custom serializer is required. Instead, the serializer is defined by the base class ([ALambdaFunction](xref:LambdaSharp.ALambdaFunction)). This change was made to ease the transition to the new [System.Text.Json](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to) serializer in a future release.

Finally, the [SerializeJson(...)](xref:LambdaSharp.ALambdaFunction.SerializeJson(System.Object)) and [DeserializeJson<T>(...)](xref:LambdaSharp.ALambdaFunction.DeserializeJson``1(System.IO.Stream)) have been marked as obsolete. Instead, use `LambdaSerializer.Serialize(...)` and `LambdaSerialize.Deserialize<T>(...)`, respectively.


## New LambdaSharp Module Syntax

### CloudWatch EventBridge Source

Lambda functions can now subscribe to a CloudWatch event bus. The [EventBus notation](~/syntax/Module-Function-Sources-EventBus.md) is straightforward. Multiple event bus subscriptions can be active at the same time.

```yaml
Sources:
  - EventBus: default
    Pattern:
      source:
        - Sample.Event
      detail-type:
        - MyEvent
      resources:
        - !Sub "lambdasharp:tier:${Deployment::Tier}"
```

### Package Build Command

The `Package` declaration now supports and optional [`Build` attribute](~/syntax/Module-Package.md), which specifies a command to run before the zip package is created. This enables creating packages from files that are generated by another project. For example, the output folder when publishing a [Blazor WebAssembly](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) application.

```yaml
- Package: MyBlazorPackage
  Build: dotnet publish -c Release MyBlazorApp
  Files: MyBlazorApp/bin/Release/netstandard2.1/publish/wwwroot/
```

### New Module Variables

New module variables have been introduced that relate to the deployment. These include `Deployment::BucketName`, `Deployment::Tier`, and `Deployment::TierLowercase`. For more details, see the [Module Global Variables](~/syntax/Module-Global-Variables.md) documentation.


## New LambdaSharp CLI Features

The CLI remains mostly unchanged from the previous release.

### Misc.
* Updated embedded CloudFormation spec to 14.3.0.
* Enhanced API Gateway V2 WebSocket logging to show error messages.
* Enabled detailed CloudWatch metrics for API Gateway deployments.
* Enhanced `lash init` to highlight deployment tier name during stack updates.
* Enhanced `lash init` for _LambdaSharp_ contributors to automatically force build and force publish.
* Enhanced `lash nuke` to only empty the deployment and logging buckets if they were created by the _LambdaSharp.Core_ module.
* Added `--no-ansi` option to `util delete-orphan-logs`, `util download-cloudformation-spec`, and `util create-invoke-methods-schema`.
* Added `util validate-assembly` command.

### Fixes
* Fixed an issue were recreating a _LambdaSharp.Core_ deployment from scratch would not update existing deployed modules with the new deployment bucket name.
* Let CloudFormation determine the name for `AWS::ApiGateway::Model` resources.
* Fixed an issue where the `--aws-region` command option didn't always work as expected.


## New LambdaSharp SDK Features

### CloudWatch Metrics

Several Lambda function base classes have been enhanced with [custom CloudWatch metrics](Metrics-Function.md) to augment existing AWS metrics. In addition, the [ALambdaFunction](xref:LambdaSharp.ALambdaFunction) base class has new [LogMetric(...)](xref:LambdaSharp.ALambdaFunction.LogMetric(System.Collections.Generic.IEnumerable{LambdaSharp.LambdaMetric})) methods to emit custom CloudWatch metrics using the [embedded metric format](https://docs.aws.amazon.com/AmazonCloudWatch/latest/monitoring/CloudWatch_Embedded_Metric_Format_Specification.html).

[The MetricSample module](https://github.com/LambdaSharp/LambdaSharpTool/tree/master/Samples/MetricSample) shows how to use the new [LogMetric(...)](xref:LambdaSharp.ALambdaFunction.LogMetric(System.String,System.Double,LambdaSharp.LambdaMetricUnit)) methods to emit custom CloudWatch metrics using the [embedded metric format](https://docs.aws.amazon.com/AmazonCloudWatch/latest/monitoring/CloudWatch_Embedded_Metric_Format_Specification.html).

### CloudWatch Events

The [ALambdaFunction](xref:LambdaSharp.ALambdaFunction) base class has a new [SendEvent(...)](xref:LambdaSharp.ALambdaFunction.SendEvent``1(System.String,System.String,``0,System.Collections.Generic.IEnumerable{System.String})) method to emit CloudWatch Events to the default event bus on [Amazon EventBridge](https://docs.aws.amazon.com/eventbridge/latest/userguide/what-is-amazon-eventbridge.html).

[The EventSample module](https://github.com/LambdaSharp/LambdaSharpTool/tree/master/Samples/EventSample) shows how to use the new [LogEvent(...)](xref:LambdaSharp.ALambdaFunction.SendEvent``1(System.String,System.String,``0,System.Collections.Generic.IEnumerable{System.String})) method to emit CloudWatch Events to the default event bus on [Amazon EventBridge](https://docs.aws.amazon.com/eventbridge/latest/userguide/what-is-amazon-eventbridge.html) method for sending CloudWatch Events.

### Debug Logging

The [ALambdaFunction](xref:LambdaSharp.ALambdaFunction) has a new [LogDebug(string format, params object[] arguments)](xref:LambdaSharp.ALambdaFunction.LogDebug(System.String,System.Object[])) method to make debug logging easier. In addition, when debug logging is enabled, the base class logs the request and response streams to CloudWatch logs. See the documentation on [Lambda Debugging](Debugging.md) for more details.


## New LambdaSharp Core Services Features

The _LambdaSharp.Core_ module now uses [Amazon Kinesis Firehose](https://aws.amazon.com/kinesis/data-firehose/) for ingesting CloudWatch Log streams. During the ingestion process, the logs are converted into [queryable JSON documents](LogRecords.md).

In addition, _LambdaSharp.Core_ now emits [operational CloudWatch events](Events.md) to the default event bus, which can be used to observe and track deployed modules.

Finally, the _LambdaSharp.Core_ module emits its [own performance metrics](Metrics-Module.md) to provide quick insight into its operations.

### Misc.

Part of this release, _LambdaSharp.Core_ functions were ported to .NET Core 3.1 with null-aware code, and the modules were published with _ReadyToRun_ for shorter cold-start times.

### Fixes
* Fixed an issue with processing the Lambda report lines in the CloudWatch logs.


## Releases

### (v0.8.0.2) - TBD

#### Fixes

* LambdaSharp CLI
    * Added support to all commands for `--no-ansi`, `--quiet`, and `--verbose` options.

### (v0.8.0.1) - 2020-05-18

#### Fixes

* LambdaSharp CLI
    * Added fixes from v0.7.0.17 release.