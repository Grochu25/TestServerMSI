# Test Server MSI (Metody Sztucznych Inteligencji)
This is another university project. 

This is backend for the application created together with [Client-msi](https://github.com/karo-fox/client-msi) for testing multiple heuristic algorithms with different fitness functions.

Application allows for adding new algorithms and fitness functions created using our interfaces i form of dll library.
## Manual
Running server expose four groups of endpoints:
- First with details of heuristic algorithms and fitness functions with info about their parameters
- Second to get and delete generated reports
- Third to add, inspect and delete dll files with algorithms and functions
- Fourth to cechk state, start, stop and resume calculations with:
  - one algoritms and many functions
  - one function and many algoritms
