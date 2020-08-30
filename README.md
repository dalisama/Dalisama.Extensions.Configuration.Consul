# Dalisama.Extensions.Configuration.Consul

Hi! As a rule of thumb, it's always better to separate your application package and your configuration, especially when working with docker or/and microservice, you need to ship them separately. One way to ship your configuration is via consul that offer you the opportunity to deliver you configuration via API.

> If you implement your own API to deliver configuration, please use this package **Dalisama.Extensions.Configuration**

## Example

I am a big believer of learning by example so:

## Get consul up and running

the easiest way to do this is by using docker:

after installing docker in your workstation, run this command to get the latest consul image:

```shell-session

$ docker pull consul

```

and to run it try:

```shell-session

$ docker run \

-d \

-p 8500:8500 \

-p 8600:8600/udp \

--name=badger \

consul agent -server -ui -node=server-1 -bootstrap-expect=1 -client=0.0.0.0

```

in your browser go to: http://localhost:8500/ui/dc1/kv to make sure that consul work right.

Now, using this command let us add our configuration to consul to use it later:

``` shell-session

curl --request PUT  --data value1  http://127.0.0.1:8500/v1/kv/Example/MyKeys/key1

curl --request PUT  --data value2  http://127.0.0.1:8500/v1/kv/Example/MyKeys/key2

curl --request PUT  --data value3  http://127.0.0.1:8500/v1/kv/Example/MyKeys/key3

curl --request PUT  --data value4  http://127.0.0.1:8500/v1/kv/Example/MyKeys/key4

```

to be sure that you did configure consul right just go to:

http://localhost:8500/ui/dc1/kv/Example/MyKeys/

>**Example/MyKeys** this is the path of the configuration in consul.

## Consume the configuration

Now you need to run this web application from your visual studio or using command line Dalisama.Extensions.Configuration.Consul.Example.

after that using your browser go to:

https://localhost:5001/API/Example

and you will get

```json
[
  {
    "key1": "value1",
    "key2": "value2",
    "key3": "value3",
    "key4": 4
  },
  {
    "key1": "value1",
    "key2": "value2",
    "key3": "value3",
    "key4": 4
  }
]
```

now let us try this command to change our configuration without exiting our application

``` shell-session

curl --request PUT  --data new_value1  http://127.0.0.1:8500/v1/kv/Example/MyKeys/key1

curl --request PUT  --data new_value2  http://127.0.0.1:8500/v1/kv/Example/MyKeys/key2

curl --request PUT  --data new_value3  http://127.0.0.1:8500/v1/kv/Example/MyKeys/key3

curl --request PUT  --data new_value4  http://127.0.0.1:8500/v1/kv/Example/MyKeys/key4

```

now let 's refresh our URL:

```json
[
  {
    "key1": "new_value1",
    "key2": "new_value2",
    "key3": "new_value3",
    "key4": 4
  },
  {
    "key1": "value1",
    "key2": "value2",
    "key3": "value3",
    "key4": 4
  }
]
```

And **voilà** the configuration gets a refreshed. To be more accurate the first element of the array did. but why?

```` csharp

[HttpGet]

public List<Keys> Get([FromServices] IOptionsSnapshot<Keys> option1, [FromServices] IOptions<Keys> option2)

{

return new List<Keys> { option1.Value, option2.Value };

}

````

You can notice that the first element is injected as **IOptionsSnapshot** that is why it did change but the second is element is injected as **IOptions** that's why it didn't change.

So how to integrate the library, simply put:

In the startup.cs:

```csharp

public Startup(IConfiguration configuration)

{

Configuration = configuration;

Configuration = (IConfiguration)new ConfigurationBuilder().AddApiConfiguration(options =>

{

options.ApiUri = "http://localhost:8500/v1/kv/";

options.KeyPath = @"Example";

options.ReloadOnChange = true;

options.IgnoreSSL = true;

}).Build();

}

```

> For authentication you need to set options.Headers with the token:
>
>  ````csharp
>options.Headers = new Dictionary<string, string>
>  {  // for authentication you can use one of those two but not both, don't be creedy
>  // ["X-Consul-Token"]="token"
>  // [Authorization]="Bearer <token>"
>  };
>  ````

Finally, you need to inject your configuration mapping:

```` csharp
services.Configure<Keys>(Configuration.GetSection("MyKeys"));
````
