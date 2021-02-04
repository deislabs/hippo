# Custom Domains

By default, a Pegasus app is available at a default domain which has the form of
`{name of app}.pegasus.local`. For example, an app named `myapp` can be reached
at `myapp.pegasus.local`.

To make your app available at a different domain, you add a custom domain to it.

You can add custom domains to any app.

AS short summary of steps to follow are:

1. Confirm you own the domain name. You can buy a custom domain at any domain
   registrar.
1. Add the custom domain to your application.
1. Look up the DNS target provided by Pegasus.
1. Configure your app's DNS provider to point to the Pegasus-supplied DNS target
1. Confirm your app is accessible via the custom domain. There might be a delay
   while changes propagate.

## Configuring DNS for Custom Domains

After you add a domain, you need to point your DNS provider at the DNS target
provided by Pegasus.

You usually configure a new CNAME record with your DNS provider to point it at
Pegasus.

The following table shows common CNAME record patterns:

| Record | Name    | Target                    |
|--------|---------|---------------------------|
| CNAME  | www     | hello-world.pegasus.local |
| CNAME  | blog    | my-blog.pegasus.local     |
| CNAME  | staging | hello-world.pegasus.local |

Consult your DNS provider's documentation for specific instructions on creating
CNAME records.

You can confirm that your DNS is configured correctly with the `host` command,
assuming your DNS changes have propagated:

```
$ host www.example.com
www.example.com is an alias for hello-world.pegasus.local.
...
```

## Configuring DNS for Root Domains

Configuring your DNS provider for a root domain is similar to configuring a DNS
provider for a subdomain. However, whereas with subdomains the type of record to
configure is always a CNAME, with root domains the type of record depends on the
DNS provider.

Whichever provider you have, point the ALIAS/ANAME/CNAME entry for your root
domain to the DNS Target, just as you would with a CNAME record:

| Record          | Name | Target                    |
|-----------------|------|---------------------------|
| ALIAS or ANAME  | @    | hello-world.pegasus.local |

## Rules on Adding Domains

Any Pegasus user can attempt to add domains to their app. Instead of explicit
domain ownership verification, Pegasus enforces the following rules to ensure
domains aren't claimed by multiple owners:

1. A given domain can only be added to a single Pegasus app. For example, if you
   add `www.example.com` to `hello-world`, you cannot add that domain to
   `goodbye-world` without removing the domain from `hello-world` first.
1. Wildcard domains are unsupported at this time.
