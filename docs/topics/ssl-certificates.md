# SSL Certificates

SSL is a cryptographic protocol that provides end-to-end encryption and
integrity for all web requests. Applications that transmit sensitive data should
enable SSL to ensure all information is transmitted securely.

## Overview

Because of the unique nature of SSL validation, provisioning SSL for your domain
is a multi-step process that involves several third-parties. You will need to:

1. Purchase an SSL certificate from your SSL provider
1. Upload the certificate to Hippo

You can verify the details of your domain's SSL configuration with `hippo certificates list`.

```
$ hippo certificates list
| Common Name     | Subject Alternate Names | Expires                  | Fingerprint     | Created     | Updated     |
|-----------------|-------------------------|--------------------------|-----------------|-------------|-------------|
| www.example.com | blog.example.com        | 31 Dec 2022 (in 2 years) | 8F:8E[...]CD:EB | 29 Jan 2021 | 30 Jan 2021 |
```

## Acquiring an SSL Certificate

Acquiring an SSL certificate varies in cost and process depending on the vendor.
[Let's Encrypt](https://letsencrypt.org/) offers a simple way to issue a
certificate and is highly recommended, as it's free and easy to get started. If
you’re able to use this provider, see [issuing an SSL certificate with Let's
Encrypt](https://letsencrypt.org/getting-started/) for instructions.

## DNS and Domain Configuration

Once the SSL certificate is provisioned and your certificate is confirmed, you
can route requests for your domain through Hippo. Unless you've already done
so, add the domain specified when generating the Certificate Signing Request to
your app with:

```
$ hippo domains add www.example.com
Adding www.example.com to example-app... done
```

Add your certificate, any intermediate certificates, and private key to the endpoint with the certs:add command.

```
hippo certificates add www.example.com server.crt server.key
Adding SSL endpoint... done
www.example.com
```

Hippo will investigate the certificate and extract any relevant information
from it such as the Common Name, Subject Alternate Names (SAN), fingerprint and
more.

## Adding a Certificate Chain

Sometimes, your certificates (such as a self-signed or a cheap certificate) need
additional certificates to establish the chain of trust. What you need to do is
bundle all the certificates into one file and give that to Hippo. Importantly,
your site’s certificate must be the first one:

```
$ cat server.crt server.ca > server.bundle
```

After that, you can add them to Hippo:

```
$ hippo certificates add www.example.com server.bundle server.key
Adding SSL endpoint... done
www.example.com
```
