Yes, {PRODUCT} has a robust RESTful API built to encourage interoperability and integrations with other services. You can perform most functions of the site through the api.

For example, we might search for events with the API by calling an endpoint like so:

```
curl --location --request GET '{URL}/api/search?latlong=62+54&distance=100'
```

You can find the full API documentation [here.](https://documenter.getpostman.com/view/8888079/SW15xbbc?version=latest)
