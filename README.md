# SteamMarketLib

Meh


## Usage

### Set currency

```csharp
Thread.CurrentThread.CurrentCulture = new CultureInfo("nl-NL");
```

### Get price for object

```csharp
SteamItemPriceData price = await SteamMarket.GetPrice(578080, "GAMESCOM INVITATIONAL CRATE");
```

or 

```csharp
SteamItemPriceData price = SteamMarket.GetPrice(578080, "GAMESCOM INVITATIONAL CRATE").Result;
```
