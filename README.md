# SteamMarketLib

Meh


## Usage

### Get price for object

```csharp
SteamItemPriceData price = await SteamMarket.GetPrice(578080, "GAMESCOM INVITATIONAL CRATE", Currency.EUR); // Currency is optional
```

or 

```csharp
SteamItemPriceData price = SteamMarket.GetPrice(578080, "GAMESCOM INVITATIONAL CRATE").Result;
```
