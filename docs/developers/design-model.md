# Understanding Hippo's design

**NOTE: As of current writing, the actual Hippo data model and flow are not quite where
this document says they are. That's because Hippo is ~a journey~ a work in progress, and
we're figuring things out as we go. This document is how we currently think things
_ought_ to work, but the code may reflect older thinking.**

## Conceptual model

Hippo is a tool for deploying _applications_.  An application is defined in a rather
circular way, as the unit of deployment for Hippo.  That is, although its code
and structure may change, and although different versions may be deployed at the
same time, an application is deployed and versioned as a single thing.

> An example of an application is a weather service or a to-do list.

The various versions of an application are stored as bindles. A bindle associated with
an application is called a _revision_.  Revisions don't do anything in themselves -
they just record that Hippo knows about the bindle and which application is belongs to.
**NOTE: We're not sure if this is what revisions model in the existing code - may need
to review this.**

> The weather application example might have bindles - and therefore revisions -
> named `contoso/weather/0.4.0`, `contoso/weather/2.0.0` and `contoso/weather/2.1.0+beta1`.

Applications are deployed via _channels_. A channel represents a particular configuration
or use case of the application. A typical use of channels is for the stages of maturity
of an application, but you can use them for any situation where you want to have
different versions or different configurations.

> The weather application might have channels named `development` (for work in progress),
> `staging` (for pre-production testing), and `production` (for the live site).
> If the weather application served different communities, it could also have
> channels such as `production/vanuatu` and `production/samoa` configured for those
> specific communities.

**NOTE: The existing code includes the concept of _releases_, which capture the pairing
of a channel and a revision. We're not sure if those are still meaningful.**

## What does a channel control?

The bulk of configuration is done at the channel level. A channel controls:

* Environment variables
  > The weather application might use a `DATA_SOURCE` environment variable to determine
  > where it gets its weather information from. In the `development` channel this could
  > be set to a static, historical data set for predictable testing. In `staging` and
  > `production` it could point to a live weather feed.
* The domain to serve on
  > You might serve the `development` channel on `weather.dev.local`, `staging` on
  > `staging.weatherexample.com` and `production` on `weatherexample.com`.
* Which revision of the application to run - see below

## Channels and revisions

Revisions are defined at application scope - they are bindles that Hippo knows are
associated with the application. However, it is up to each channel which revision
it actually uses. A channel may choose in one of two ways:

* Rules based (auto deploy). The channel will serve the most recent revision that
  meets its constraints. Here 'recent' is defined as the highest version number.
  **NOTE: We have not yet fully defined how prerelease segments interact with
  this.**
  > The `production` channel might be configured to serve "the most recent
  > revision whose minor version is 1.1". If the application had revisions
  > 1.1.0, 1.1.1 and 1.2.0, `production` would serve 1.1.1. If you added
  > revision 1.1.2, then `production` would automatically switch to that.
* Specific revision. The channel will serve the specific revision given.
  > The `production` channel might be configured to serve revision 1.1.1,
  > and require a manual update to switch it over to a new revision.

**NOTE: At the database level we can't easily express this either-or choice. We
currently expect to have a `rules` column and a `revision` column, both nullable,
and a `revision_selection_mode` column that says which to use. The application
has to enforce that the appropriate column is not NULL.**

A channel always has a _current revision_ - the revision being served, whether
specific or derived from the rules.

## Snapshots

A _snapshot_ represents what a channel was serving at a particular time.
Configuration such as domain and environment variables is copied when the
snapshot is taken. The snapshot also copies the channel's _current_ revision.

If you roll a channel back to a snapshot, the snapshot's configuration overwrites
the channel's configuration, and the channel is set to serve the specific
revision in the snapshot. If the channel was rules-based, it stops being
rules based; it is a manual action to set it back to being rules-based.
