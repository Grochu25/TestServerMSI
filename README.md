# Test Server MSI (Metody Sztucznych Inteligencji)
This is another university project. 

This is backend for the application created together with [Client-msi](https://github.com/karo-fox/client-msi) for testing multiple heuristic algorithms with different fitness functions.

The application allows adding new algorithms and fitness functions created using our interfaces in the form of a DLL library.
## Manual
Running server exposes four groups of endpoints:
- The first provides details of heuristic algorithms and fitness functions with info about their parameters
- The second allows retrieving and deleting generated reports
- The third enables adding, inspecting, and deleting dll files with algorithms and functions
- The fourth allows checking the state, start, stop and resume calculations with:
  - one algorithm and many functions
  - one function and many algorithms
