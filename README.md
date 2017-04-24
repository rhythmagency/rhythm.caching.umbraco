# Introduction

Builds upon [Rhythm.Caching.Core](https://github.com/rhythmagency/rhythm.caching.core) with tools to invalidate caches
based on events in Umbraco.

Refer to the [generated documentation](docs/generated.md) for more details.

# Installation

Install with NuGet. Search for "Rhythm.Caching.Umbraco".

# Overview

## CacheHelper

* **PreviewCacheKeys** The cache keys you can use to separate the caches for live and preview content.

## Invalidators

* **InvalidatorByPage** When a content node is changed, this invalidates a cache by the node's ID.
* **InvalidatorByPageAliases** When a content node matching any of the specified content type aliases is published, this invalidates a cache.
* **InvalidatorByParentPage** When a content node is changed, this invalidates a cache by the ID of that node's parent node.

# Maintainers

To create a new release to NuGet, see the [NuGet documentation](docs/nuget.md).