# Custom Domains

By default, a Hippo app is available at a default domain which has the form of
`{name of app}.hippo.test`. For example, an app named `myapp` can be reached
at `myapp.hippo.test`.

To make your app available at a different domain, you add a custom domain to it.

You can add custom domains to any app.

A short summary of steps to follow are:

1. Confirm you own the domain name. You can buy a custom domain at any domain
   registrar.
1. Add the custom domain to your application.
1. Look up the DNS target provided by Hippo.
1. Configure your app's DNS provider to point to the Hippo-supplied DNS target
1. Confirm your app is accessible via the custom domain. There might be a delay
   while changes propagate.

## Configuring DNS for Custom Domains

After you add a domain, you need to point your DNS provider at the DNS target
provided by Hippo.

You usually configure a new CNAME record with your DNS provider to point it at
Hippo.

The following table shows common CNAME record patterns:

| Record | Name    | Target                    |
|--------|---------|---------------------------|
| CNAME  | www     | hello-world.hippo.test    |
| CNAME  | blog    | my-blog.hippo.test        |
| CNAME  | staging | hello-world.hippo.test    |

Consult your DNS provider's documentation for specific instructions on creating
CNAME records.

You can confirm that your DNS is configured correctly with the `host` command,
assuming your DNS changes have propagated:

```
$ host www.example.com
www.example.com is an alias for hello-world.hippo.test.
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
| ALIAS or ANAME  | @    | hello-world.hippo.test    |

## Rules on Adding Domains

Any Hippo user can attempt to add domains to their app. Instead of explicit
domain ownership verification, Hippo enforces the following rules to ensure
domains aren't claimed by multiple owners:

1. A given domain can only be added to a single Hippo app. For example, if you
   add `www.example.com` to `hello-world`, you cannot add that domain to
   `goodbye-world` without removing the domain from `hello-world` first.
1. Wildcard domains are unsupported at this time.
