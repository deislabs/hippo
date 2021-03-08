# Developer guide

This guide explains how to set up your environment for developing Hippo.

## System Requirements

To build Hippo, you will only need the latest stable version of Python 3.

However, if you want to run Hippo as a development server or run the test suite,
you will need to be running a Linux server with the following tools installed:

- `systemctl`
- `wagi`
- `traefik`
- `postgresql`

## Building

```console
$ virtualenv venv
$ source venv/bin/activate
$ pip install -r requirements.txt
```

## Running

```console
$ python manage.py migrate
$ python manage.py runserver
```

## Testing

```console
$ python manage.py test
```
