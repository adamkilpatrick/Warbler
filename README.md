# Warbler
Simple Tape Wobble VST

## User Guide
There are 3 paramaters in this VST:
- Depth:
  - The amplitude of the oscillating tape head.
- Frequency
  - The frequency that the tapehead is osciallating at (oscillation here is sin based).
- Manual depth:
  - A manual offset added to the calculated oscillating depth from the two above parameters. This is useful if you want to do a non-sinusoidal oscillation, since you can set the "Depth" param to 0 and assign this param to an LFO (or other autoamtion source) of your choosing. It can also be used to add some noise into the oscillation provided from the two above params.


## Build Guide
```
dotnet publish -c Release -r win-x64 -p:PublishAot=true
```

## TODOs
- Automate the building of releases
- Build/release non-windows targets
- Choose better limits for parameters

## References
- Built on NPlug: https://github.com/xoofx/NPlug/ 
