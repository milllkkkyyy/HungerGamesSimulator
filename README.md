# Hunger Games Simulator

A project created to get experience with Blazor, ASP.NET, C#, Testing, and Reflection. I started and will finish this product for Q4. 
In the project, I use testing suites to ensure functionality before implementing into the codebase.

## Creating Designer Strings

Basic Service.json Template

```
[ 
  {
    "Contexts": [ "Combat" ],
    "Inputs": [
      {
        "Type": "Tribute",
        "Requirements": [ "HasWeapon=Bow", "!IsDead" ]
      },
      {
        "Type": "Tribute",
        "Requirements": [ "IsDead" ]
      }
    ],
    "Texts": [
      "{input[0]} shoots an arrow in the chest of {input[1]} killing him swiftly"
    ]
  }
]
```

When you create a new json for a service you must add it to the Files.Json in the same folder

### Contexts

An array of contexts which describe what the string should be used for. These need to be implemented in the codebase in order to be used in the designer string system.

### Inputs

An array of objects ("inputs") which each must have a specific type. They also can have an optional list of requirements 

Currently implemented types (with implemented requirements):
* Tribute ( IsDead, HasWeapon=OptionalWeaponName, IsInParty )
* Party
* Int
* CornucopiaTribute ( HasAdvantage, HasDisadvantage )

With requirements you can negate them by inserting a "!" infront of the requirement string

### Texts

An array of strings which can be used to create dynamic strings at runtime. 

Format:
* Use input[index] to inform which inputs go where
* Can use properties of input types ( e.g. if input[0] is type Tribute, you can use input[0].Weapon)
* Requires the input to be wrapped into a block "{input[0]}.."

#### Properties that can be used

* Tribute: Weapon, Health
* Party: Dead, Alive
