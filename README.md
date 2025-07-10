# SynapseAssessment
# Signal Booster Assignment

📘 Scenario

You’ve inherited a core utility from a developer who believed in “moving fast and breaking things.” The tool reads a physician’s note, extracts relevant information about the patient’s durable medical equipment (DME) needs — such as CPAPs or oxygen tanks — and sends the structured data to an external API.

Unfortunately, this developer took minimalism to an extreme:
- All logic is packed into `Main`
- Variable names are cryptic and inconsistent
- The code includes misleading comments and unused logic
- There’s no logging, no error handling, and no unit tests

Now, it’s your responsibility to clean it up. The business needs this feature to be reliable, maintainable, and production-ready and they need it fast.


🧪 Your Mission

Refactor the provided code into something that’s understandable, testable, and maintainable. Specifically:

1. **Refactor the logic into well-named, testable methods**
   - Improve structure and readability
   - Remove redundant or dead code
   - Use clear and consistent naming
  
**Implemented as per requirement**

2. **Introduce logging and basic error handling**
   - Avoid swallowing exceptions
   - Log meaningful steps for observability
  
**Implemented as per requirement**

3. **Write at least one unit test**
   - Show how you’d test a meaningful part of the logic
  
**Implemented as per requirement**

4. **Replace misleading or unclear comments with helpful ones**

**Implemented as per requirement**

6. **Keep it functional**
   - Your version must still:
     - Read a physician note from a file
     - Extract structured data (device type, provider, etc.)
     - POST the data to `https://alert-api.com/DrExtract` (Not a real link)

**Implemented as per requirement**

7. **(Optional stretch goals)**
   - Replace the manual extraction logic with an LLM (e.g., OpenAI or Azure OpenAI)
   - Accept multiple input formats (e.g., JSON-wrapped notes)
   - Add configurability for file path or API endpoint
   - Support more DME device types or qualifiers

**Implemented as per requirement in a new class SignalBooster_Original_WithLLM.cs**

📄 README Implementation

Please include a short `README.md` file in your submission with the following:

- What IDE or tools you used (e.g., VS Code, Rider, Visual Studio):
- ** Visual Studio and Visual Studio Code

- Whether you used any AI development tools (e.g., GitHub Copilot, Cursor, Cody):
- ** Yes, Github Copilot
- 
- Any assumptions, limitations, or future improvements
- Assumptions:
•	The physician note is either in plain text and stored in a file, or a default note is used.
•	Device types, mask types, add-ons, and qualifiers are identified by simple keyword matching.
•	The provider’s name always starts with "Dr." and is at the end of the note.
•	Only a limited set of device types ("CPAP", "Oxygen Tank", "Wheelchair") and qualifiers (e.g., "AHI > 20") are supported.
- Limitations:
•	Extraction logic relies on exact keywords and may miss variations or more complex phrasing.
•	Only supports a small, hardcoded set of device types, add-ons, and qualifiers.
•	Does not handle multiple devices or complex notes with more than one order.
•	The provider extraction assumes a specific format and may fail for other naming conventions.
•	No support for structured input formats (e.g., JSON-wrapped notes).
•	The API endpoint and file path are not easily configurable at runtime.
- Future Improvements:
•	Use a language model (LLM) or NLP library for more robust and flexible information extraction.(Added in a new class)
•	Accept multiple input formats i.e. json
•	Make file paths and API endpoints configurable via parameters or configuration files.
•	Support a broader range of device types, add-ons, and qualifiers.
•	Add unit tests and error handling for edge cases.
•	Refactor HTTP calls to be asynchronous for better performance.
•	Improve provider name extraction to handle more formats and edge cases.
•	Add logging and telemetry for better monitoring and diagnostics.

**Code can be run using dotnet run command in **Visual Studio Code** or through Run the solution in Visual Studio/Code. 
All methods run successfully; SendPayloadToApi method will throw hostname not found as uning a fake url (https://alert-api.com/DrExtract) as per requirement.
SignalBooster_Original_WithLLM.cs file added to utilize LLM instead of manual instruction.**
