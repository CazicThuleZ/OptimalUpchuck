========================
CODE SNIPPETS
========================
TITLE: Running Examples via Command-Line
DESCRIPTION: Shows how to use test filters with the `dotnet test` command to run specific examples from the command line.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithAgents/README.md#_snippet_24

LANGUAGE: bash
CODE:
```
dotnet test --filter Step3_Chat
```

----------------------------------------

TITLE: Running Examples via Command-Line
DESCRIPTION: Provides instructions on how to execute Semantic Kernel examples using the .NET CLI and the `--filter` option to target specific tests.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithAgents/README.md#_snippet_36

LANGUAGE: bash
CODE:
```
dotnet test --filter "Category=Orchestration"

```

LANGUAGE: bash
CODE:
```
dotnet test --filter "Name=Step01_Concurrent"

```

LANGUAGE: bash
CODE:
```
dotnet test --filter "FullyQualifiedName~Orchestration.Step02_Sequential"

```

----------------------------------------

TITLE: Create and Interact with BedrockAgent (C#)
DESCRIPTION: Provides a basic example of creating a Bedrock Agent and interacting with it. This covers the fundamental setup for using Bedrock agents.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithAgents/README.md#_snippet_18

LANGUAGE: cs
CODE:
```
using Microsoft.SemanticKernel.Agents.Bedrock;
using Microsoft.SemanticKernel.Connectors.Bedrock;

// ... other usings

public class Step01_BedrockAgent
{
    public static async Task RunAsync(IKernel kernel)
    {
        // Configure Bedrock client
        var bedrockClient = new BedrockClient(
            new BedrockClientConfig { AwsAccessKeyId = "YOUR_AWS_ACCESS_KEY_ID", AwsSecretAccessKey = "YOUR_AWS_SECRET_ACCESS_KEY", Region = "us-east-1" });

        // Create a Bedrock Agent
        var bedrockAgent = kernel.CreateBedrockAgent(
            bedrockClient,
            "your-bedrock-agent-id", // Bedrock Agent ID
            "MyBedrockAgent",
            "This is a Bedrock Agent."
        );

        // Interact with the Bedrock Agent
        var response = await bedrockAgent.InvokeAsync("Tell me about Amazon rainforest.");
        Console.WriteLine(response);
    }
}
```

----------------------------------------


TITLE: Running Examples with Filters
DESCRIPTION: Demonstrates how to run specific Semantic Kernel examples from the command line using test filters. This is useful for isolating and testing particular functionalities or components of the project.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithProcesses/README.md#_snippet_15

LANGUAGE: cli
CODE:
```
dotnet test --filter Step01_Processes
```

----------------------------------------

TITLE: Simple Process Flowchart
DESCRIPTION: Illustrates a basic sequential process flow with 'Start', 'DoSomeWork', 'DoMoreWork', and 'End' steps. This serves as a foundational example for understanding process execution.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithProcesses/README.md#_snippet_0

LANGUAGE: mermaid
CODE:
```
flowchart LR  
    Start(Start)--> DoSomeWork(DoSomeWork)
    DoSomeWork--> DoMoreWork(DoMoreWork)
    DoMoreWork--> End(End)
```

----------------------------------------

TITLE: Create and Use an Agent (C#)
DESCRIPTION: Demonstrates the fundamental steps to create and interact with a Semantic Kernel agent. This example serves as the initial entry point for understanding agent creation.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithAgents/README.md#_snippet_0

LANGUAGE: cs
CODE:
```
using Microsoft.SemanticKernel.Agents.Abstractions;

// ... other usings

public class Step01_Agent
{
    public static async Task RunAsync(IKernel kernel)
    {
        // Create an agent
        var agent = kernel.CreateAgent(
            "MyAwesomeAgent",
            "This is a very awesome agent.",
            "This agent is designed to be helpful and creative."
        );

        // Interact with the agent
        var response = await agent.InvokeAsync("Hello, agent!");
        Console.WriteLine(response);
    }
}
```

----------------------------------------


TITLE: Copilot Studio Agent Examples
DESCRIPTION: Demonstrates basic interaction with CopilotStudioAgent, using it with an AgentThread, and enabling web search functionality.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithAgents/README.md#_snippet_22

LANGUAGE: cs
CODE:
```
CopilotStudioAgent/Step01_CopilotStudioAgent.cs
```

LANGUAGE: cs
CODE:
```
CopilotStudioAgent/Step02_CopilotStudioAgent_Thread.cs
```

LANGUAGE: cs
CODE:
```
CopilotStudioAgent/Step03_CopilotStudioAgent_WebSearch.cs
```

----------------------------------------

TITLE: Orchestration Examples
DESCRIPTION: Illustrates various orchestration patterns including concurrent, sequential, group chat, and handoff orchestrations, with examples for structured output and human-in-the-loop scenarios.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithAgents/README.md#_snippet_23

LANGUAGE: cs
CODE:
```
Orchestration/Step01_Concurrent.cs
```

LANGUAGE: cs
CODE:
```
Orchestration/Step01a_ConcurrentWithStructuredOutput.cs
```

LANGUAGE: cs
CODE:
```
Orchestration/Step02_Sequential.cs
```

LANGUAGE: cs
CODE:
```
Orchestration/Step02a_Sequential.cs
```

LANGUAGE: cs
CODE:
```
Orchestration/Step03_GroupChat.cs
```

LANGUAGE: cs
CODE:
```
Orchestration/Step03a_GroupChatWithHumanInTheLoop.cs
```

LANGUAGE: cs
CODE:
```
Orchestration/Step03b_GroupChatWithAIManager.cs
```

LANGUAGE: cs
CODE:
```
Orchestration/Step04_Handoff.cs
```

LANGUAGE: cs
CODE:
```
Orchestration/Step04b_HandoffWithStructuredInput.cs
```

----------------------------------------

TITLE: Load and Run Plugin
DESCRIPTION: Loads a plugin from a specified directory and invokes a function within that plugin. It demonstrates how to construct arguments and execute a Semantic Kernel function, then prints the result.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/notebooks/00-getting-started.ipynb#_snippet_4

LANGUAGE: C#
CODE:
```
// FunPlugin directory path
var funPluginDirectoryPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..", "..", "prompt_template_samples", "FunPlugin");

// Load the FunPlugin from the Plugins Directory
var funPluginFunctions = kernel.ImportPluginFromPromptDirectory(funPluginDirectoryPath);

// Construct arguments
var arguments = new KernelArguments() { ["input"] = "time travel to dinosaur age" };

// Run the Function called Joke
var result = await kernel.InvokeAsync(funPluginFunctions["Joke"], arguments);

// Return the result to the Notebook
Console.WriteLine(result);
```

----------------------------------------

TITLE: Instantiate Kernel Builder
DESCRIPTION: Initializes the Semantic Kernel builder, which is the starting point for configuring and creating a Kernel instance. It sets up the necessary components for AI service integration.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/notebooks/00-getting-started.ipynb#_snippet_2

LANGUAGE: C#
CODE:
```
using Microsoft.SemanticKernel;
using Kernel = Microsoft.SemanticKernel.Kernel;

//Create Kernel builder
var builder = Kernel.CreateBuilder();
```

----------------------------------------




TITLE: C# Example: Running Prompts with Input Parameters
DESCRIPTION: Demonstrates setting up Semantic Kernel in a C# console application to execute prompts with dynamic input. It shows how to configure Azure OpenAI or OpenAI chat completion and invoke a summarization prompt with different text inputs.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/README.md#_snippet_0

LANGUAGE: csharp
CODE:
```
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var builder = Kernel.CreateBuilder();

builder.AddAzureOpenAIChatCompletion(
         "gpt-35-turbo",                      // Azure OpenAI Deployment Name
         "https://contoso.openai.azure.com/", // Azure OpenAI Endpoint
         "...your Azure OpenAI Key...");      // Azure OpenAI Key

// Alternative using OpenAI
//builder.AddOpenAIChatCompletion(
//         "gpt-3.5-turbo",                  // OpenAI Model name
//         "...your OpenAI API Key...");     // OpenAI API Key

var kernel = builder.Build();

var prompt = @"{{$input}}

One line TLDR with the fewest words.";

var summarize = kernel.CreateFunctionFromPrompt(prompt, executionSettings: new OpenAIPromptExecutionSettings { MaxTokens = 100 });

string text1 = @"1st Law of Thermodynamics - Energy cannot be created or destroyed.
2nd Law of Thermodynamics - For a spontaneous process, the entropy of the universe increases.
3rd Law of Thermodynamics - A perfect crystal at zero Kelvin has zero entropy.";

string text2 = @"1. An object at rest remains at rest, and an object in motion remains in motion at constant speed and in a straight line unless acted on by an unbalanced force.
2. The acceleration of an object depends on the mass of the object and the amount of force applied.
3. Whenever one object exerts a force on another object, the second object exerts an equal and opposite on the first.";

Console.WriteLine(await kernel.InvokeAsync(summarize, new() { ["input"] = text1 }));

Console.WriteLine(await kernel.InvokeAsync(summarize, new() { ["input"] = text2 }));

// Output:
//   Energy conserved, entropy increases, zero entropy at 0K.
//   Objects move in response to forces.

```

----------------------------------------

TITLE: Create an Azure AI Agent (C#)
DESCRIPTION: Details the steps to create an agent that utilizes Azure AI services. This example focuses on the basic creation of an Azure AI agent.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithAgents/README.md#_snippet_12

LANGUAGE: cs
CODE:
```
using Microsoft.SemanticKernel.Agents.AzureAI;
using Microsoft.SemanticKernel.Connectors.AzureAI;

// ... other usings

public class Step01_AzureAIAgent
{
    public static async Task RunAsync(IKernel kernel)
    {
        // Configure Azure OpenAI client
        var azureOpenAIClient = new AzureOpenAIClient(
            new AzureOpenAIClientConfig
            {
                DeploymentName = "your-deployment-name",
                Endpoint = "https://your-resource.openai.azure.com/",
                ApiKey = "YOUR_AZURE_OPENAI_API_KEY"
            });

        // Create an Azure AI Agent
        var azureAIAgent = kernel.CreateAzureAIAssistantAgent(
            azureOpenAIClient,
            "asst_xxxxxxxxxxxxxxxxxxxx", // Assistant ID
            "MyAzureAIAgent",
            "This is an Azure AI Agent."
        );

        // Interact with the Azure AI Agent
        var response = await azureAIAgent.InvokeAsync("What is the capital of France?");
        Console.WriteLine(response);
    }
}
```

----------------------------------------





TITLE: Configure AI Service Credentials
DESCRIPTION: Configures the Semantic Kernel with AI service credentials, supporting both Azure OpenAI and OpenAI. It loads settings from a file and adds the appropriate completion service to the builder.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/notebooks/00-getting-started.ipynb#_snippet_3

LANGUAGE: C#
CODE:
```
// Configure AI service credentials used by the kernel
var (useAzureOpenAI, model, azureEndpoint, apiKey, orgId) = Settings.LoadFromFile();

if (useAzureOpenAI)
    builder.AddAzureOpenAIChatCompletion(model, azureEndpoint, apiKey);
else
    builder.AddOpenAIChatCompletion(model, apiKey, orgId);

var kernel = builder.Build();
```

----------------------------------------

TITLE: Install CopilotStudio NuGet Package
DESCRIPTION: Installs the `Microsoft.SemanticKernel.Agents.CopilotStudio` package using the .NET CLI. This command is used to add the necessary library for integrating with Copilot Studio agents.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/src/Agents/Copilot/README.md#_snippet_0

LANGUAGE: bash
CODE:
```
dotnet add package Microsoft.SemanticKernel.Agents.CopilotStudio --prerelease
```

----------------------------------------




TITLE: Configuring Secrets with .NET Secret Manager
DESCRIPTION: Guides users on setting up API keys and credentials for various AI services (OpenAI, Azure OpenAI, Azure AI, Bedrock) using the .NET Secret Manager tool.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithAgents/README.md#_snippet_27

LANGUAGE: bash
CODE:
```
cd dotnet/samples/GettingStartedWithAgents
```

LANGUAGE: bash
CODE:
```
dotnet user-secrets list
```

LANGUAGE: bash
CODE:
```
dotnet user-secrets init
```

LANGUAGE: bash
CODE:
```
dotnet user-secrets set "OpenAI:ChatModelId" "..."
```

LANGUAGE: bash
CODE:
```
dotnet user-secrets set "OpenAI:ApiKey" "..."
```

LANGUAGE: bash
CODE:
```
dotnet user-secrets set "AzureOpenAI:ChatDeploymentName" "gpt-4o"
```

LANGUAGE: bash
CODE:
```
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://... .openai.azure.com/"
```

LANGUAGE: bash
CODE:
```
dotnet user-secrets set "AzureOpenAI:ApiKey" "..."
```

LANGUAGE: bash
CODE:
```
dotnet user-secrets set "AzureAI:Endpoint" "..."
```

LANGUAGE: bash
CODE:
```
dotnet user-secrets set "AzureAI:ChatModelId" "gpt-4o"
```

LANGUAGE: bash
CODE:
```
dotnet user-secrets set "BedrockAgent:AgentResourceRoleArn" "arn:aws:iam::...:role/..."
```

LANGUAGE: bash
CODE:
```
dotnet user-secrets set "BedrockAgent:FoundationModel" "..."
```

----------------------------------------



TITLE: Potato Fries Preparation Flowchart
DESCRIPTION: Visualizes the process for preparing potato fries. It outlines the steps from gathering ingredients and cutting potatoes to frying the food, including event triggers for preparation start and completion, and a failure loop for ruined food.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithProcesses/README.md#_snippet_7

LANGUAGE: mermaid
CODE:
```
flowchart LR
    PreparePotatoFriesEvent([Prepare Potato <br/> Fries Event])
    PotatoFriesReadyEvent([Potato Fries <br/> Ready Event])

    GatherIngredientsStep[Gather Ingredients <br/> Step]
    CutStep[Cut Food <br/> Step]
    FryStep[Fry Food <br/> Step]

    PreparePotatoFriesEvent --> GatherIngredientsStep -->| Slice Potatoes <br/> _Ingredients Gathered_ | CutStep --> |**Potato Sliced Ready** <br/> _Food Sliced Ready_ | FryStep --> |_Fried Food Ready_|PotatoFriesReadyEvent
    FryStep -->|Fried Potato Ruined <br/> _Fried Food Ruined_| GatherIngredientsStep
```

----------------------------------------

TITLE: Account Opening Process Steps
DESCRIPTION: Outlines the conceptual steps involved in an account opening process using Semantic Kernel. This includes application form filling, credit score verification, fraud detection, and customer notification.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithProcesses/README.md#_snippet_2

LANGUAGE: csharp
CODE:
```
// Conceptual steps for account opening process:
// 1. Fill New User Account Application Form
// 2. Verify Applicant Credit Score
// 3. Apply Fraud Detection Analysis to the Application Form
// 4. Create New Entry in Core System Records
// 5. Add new account to Marketing Records
// 6. CRM Record Creation
// 7. Mail user a notification about:
//    - Failure to open a new account due to Credit Score Check
//    - Failure to open a new account due to Fraud Detection Alert
//    - Welcome package including new account details
```

----------------------------------------

TITLE: Import Semantic Kernel SDK
DESCRIPTION: Imports the Semantic Kernel SDK from NuGet, specifying the version. This is a prerequisite for using Semantic Kernel functionalities in a C# environment.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/notebooks/00-getting-started.ipynb#_snippet_1

LANGUAGE: C#
CODE:
```
#r "nuget: Microsoft.SemanticKernel, 1.23.0"
```

----------------------------------------


TITLE: Configuring Secrets with .NET Secret Manager
DESCRIPTION: Provides instructions for configuring secrets and credentials for Semantic Kernel examples using the .NET Secret Manager tool. This method helps prevent sensitive information like API keys from being leaked into the repository.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithProcesses/README.md#_snippet_16

LANGUAGE: cli
CODE:
```
cd dotnet/samples/GettingStartedWithProcesses
```

LANGUAGE: cli
CODE:
```
dotnet user-secrets list
```

LANGUAGE: cli
CODE:
```
dotnet user-secrets init
```

LANGUAGE: cli
CODE:
```
dotnet user-secrets set "OpenAI:ChatModelId" "..."
```

LANGUAGE: cli
CODE:
```
dotnet user-secrets set "OpenAI:ApiKey" "..."
```

LANGUAGE: cli
CODE:
```
dotnet user-secrets set "AzureOpenAI:DeploymentName" "..."
```

LANGUAGE: cli
CODE:
```
dotnet user-secrets set "AzureOpenAI:ChatDeploymentName" "..."
```

LANGUAGE: cli
CODE:
```
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://... .openai.azure.com/"
```

LANGUAGE: cli
CODE:
```
dotnet user-secrets set "AzureOpenAI:ApiKey" "..."
```

----------------------------------------

TITLE: Refactored Account Opening Flowchart (Step02b)
DESCRIPTION: Presents a refactored account opening process, grouping common steps into subprocesses. It highlights the main flow from user interaction to verification and creation, referencing reusable subprocesses.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithProcesses/README.md#_snippet_4

LANGUAGE: mermaid
CODE:
```
flowchart LR
    User(User)
    FillForm(Chat With User <br/> to Fill New <br/> Customer Form)
    NewAccountVerification[[New Account Verification<br/> Process]]
    NewAccountCreation[[New Account Creation<br/> Process]]
    Mailer(Mail <br/> Service)

    User<-->|Provides user details|FillForm
    FillForm-->|New User Form|NewAccountVerification
    NewAccountVerification-->|Account Credit Check<br/> Verification Failed|Mailer
    NewAccountVerification-->|Account Fraud<br/> Detection Failed|Mailer
    NewAccountVerification-->|Account Verification <br/> Succeeded|NewAccountCreation
    NewAccountCreation-->|Account Creation <br/> Succeeded|Mailer
```

----------------------------------------




TITLE: Configuring OpenAI Secrets with .NET Secret Manager
DESCRIPTION: Guides users on setting up OpenAI API keys and model IDs using the .NET Secret Manager tool. This is a recommended practice for securely managing credentials.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithAgents/README.md#_snippet_37

LANGUAGE: bash
CODE:
```
cd dotnet/samples/GettingStartedWithAgents

```

LANGUAGE: bash
CODE:
```
dotnet user-secrets list

```

LANGUAGE: bash
CODE:
```
dotnet user-secrets init

```

LANGUAGE: bash
CODE:
```
dotnet user-secrets set "OpenAI:ChatModelId" "gpt-4o"

```

LANGUAGE: bash
CODE:
```
dotnet user-secrets set "OpenAI:ApiKey" "YOUR_OPENAI_API_KEY"

```

----------------------------------------



TITLE: Start Qdrant Instance with Docker
DESCRIPTION: Starts a Qdrant vector database instance using Docker. This command uses default configurations and maps the necessary ports for access.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Demos/VectorStoreRAG/README.md#_snippet_6

LANGUAGE: docker
CODE:
```
docker run -d --name qdrant -p 6333:6333 -p 6334:6334 qdrant/qdrant:latest
```

----------------------------------------



TITLE: Associate Plugins with OpenAI Assistant Agent (C#)
DESCRIPTION: Explains how to integrate plugins with an OpenAI Assistant agent, enabling it to utilize custom tools or external services.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithAgents/README.md#_snippet_8

LANGUAGE: cs
CODE:
```
using Microsoft.SemanticKernel.Agents.OpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Core;

// ... other usings

public class Step02_Assistant_Plugins
{
    public static async Task RunAsync(IKernel kernel)
    {
        // Configure OpenAI client
        var openAIClient = new OpenAIClient(
            new OpenAIClientConfig { ApiKey = "YOUR_OPENAI_API_KEY" });

        // Create an OpenAI Assistant agent
        var assistantAgent = kernel.CreateOpenAIAssistantAgent(
            openAIClient,
            "asst_xxxxxxxxxxxxxxxxxxxx", // Assistant ID
            "MyOpenAIAssistantWithPlugins",
            "This OpenAI Assistant agent uses plugins."
        );

        // Load and add a plugin to the agent
        var timePlugin = kernel.CreatePluginFromPromptDirectory("Plugins/TimePlugin");
        assistantAgent.AddPlugin(timePlugin);

        // Interact with the agent, which can now use the plugin
        var response = await assistantAgent.InvokeAsync("What time is it?");
        Console.WriteLine(response);
    }
}
```

----------------------------------------

TITLE: Use File Search Tool with OpenAI Assistant (C#)
DESCRIPTION: Demonstrates how to integrate the File Search tool with an OpenAI Assistant agent, enabling it to access and search through provided files.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithAgents/README.md#_snippet_11

LANGUAGE: cs
CODE:
```
using Microsoft.SemanticKernel.Agents.OpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// ... other usings

public class Step05_AssistantTool_FileSearch
{
    public static async Task RunAsync(IKernel kernel)
    {
        // Configure OpenAI client
        var openAIClient = new OpenAIClient(
            new OpenAIClientConfig { ApiKey = "YOUR_OPENAI_API_KEY" });

        // Create an OpenAI Assistant agent with File Search tool enabled
        var assistantAgent = kernel.CreateOpenAIAssistantAgent(
            openAIClient,
            "asst_xxxxxxxxxxxxxxxxxxxx", // Assistant ID
            "MyFileSearchAssistant",
            "This OpenAI Assistant agent uses the File Search tool."
        );

        // Upload a file to be used by the File Search tool (requires Assistant setup)
        // var fileId = await assistantAgent.UploadFileAsync("path/to/your/document.txt", "text/plain");

        // Interact with the agent, asking questions about the file content
        var response = await assistantAgent.InvokeAsync("Summarize the content of the uploaded document.");
        Console.WriteLine(response);
    }
}
```

----------------------------------------

TITLE: Account Opening Flowchart (Step02a)
DESCRIPTION: Visualizes the initial user account opening process. It details user interaction for filling forms, assistant messages, credit checks, fraud detection, and integration with core systems, marketing, CRM, and a welcome packet service.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithProcesses/README.md#_snippet_3

LANGUAGE: mermaid
CODE:
```
flowchart LR  
    User(User) -->|Provides user details| FillForm(Fill New <br/> Customer <br/> Form)  

    FillForm -->|Need more info| AssistantMessage(Assistant <br/> Message)
    FillForm -->|Welcome Message| AssistantMessage
    FillForm --> CompletedForm((Completed Form))
    AssistantMessage --> User
  
    CompletedForm --> CreditCheck(Customer <br/> Credit Score <br/> Check)
    CompletedForm --> Fraud(Fraud Detection)
    CompletedForm -->|New Customer Form + Conversation Transcript| CoreSystem
  
    CreditCheck -->|Failed - Notify user about insufficient credit score| Mailer(Mail <br/> Service)
    CreditCheck -->|Approved| Fraud  
  
    Fraud --> |Failed - Notify user about failure to confirm user identity| Mailer  
    Fraud --> |Passed| CoreSystem(Core System <br/> Record <br/> Creation)  
  
    CoreSystem --> Marketing(New Marketing <br/> Record Creation)
    CoreSystem --> CRM(CRM Record <br/> Creation)
    CoreSystem -->|Account Details| Welcome(Welcome <br/> Packet)  
  
    Marketing -->|Success| Welcome  
    CRM -->|Success| Welcome  
  
    Welcome -->|Success: Notify User about Account Creation| Mailer  
    Mailer -->|End of Interaction| User
```

----------------------------------------

TITLE: Install Semantic Kernel with Pip
DESCRIPTION: Installs the Semantic Kernel library for Python using pip. This is a prerequisite for running the examples and requires Python 3.10 or above.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/python/samples/learn_resources/README.md#_snippet_0

LANGUAGE: bash
CODE:
```
pip install semantic-kernel
```

----------------------------------------

TITLE: Vector Store Configuration Example
DESCRIPTION: Example JSON structure for configuring vector stores, specifically showing the default Azure AI Search setup within `appsettings.json`.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Demos/AgentFrameworkWithAspire/README.md#_snippet_6

LANGUAGE: json
CODE:
```
{
  "VectorStores": {
    "AzureAISearch": {
    }
  },
  "Rag": {
    "VectorStoreType": "AzureAISearch"
  }
}
```

----------------------------------------

TITLE: Enable Tracing for BedrockAgent (C#)
DESCRIPTION: Provides guidance on enabling tracing for a Bedrock Agent to inspect its chain of thoughts and execution flow.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithAgents/README.md#_snippet_21

LANGUAGE: cs
CODE:
```
using Microsoft.SemanticKernel.Agents.Bedrock;
using Microsoft.SemanticKernel.Connectors.Bedrock;
using Microsoft.Extensions.Logging;

// ... other usings

public class Step04_BedrockAgent_Trace
{
    public static async Task RunAsync(IKernel kernel)
    {
        // Configure Bedrock client
        var bedrockClient = new BedrockClient(
            new BedrockClientConfig { AwsAccessKeyId = "YOUR_AWS_ACCESS_
```

----------------------------------------

TITLE: Configuring Bedrock Secrets with .NET Secret Manager
DESCRIPTION: Guides on setting AWS Bedrock agent resource role ARN and foundation model using .NET Secret Manager for AWS Bedrock integration.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithAgents/README.md#_snippet_40

LANGUAGE: bash
CODE:
```
dotnet user-secrets set "BedrockAgent:AgentResourceRoleArn" "arn:aws:iam::123456789012:role/YourBedrockAgentRole"

```

LANGUAGE: bash
CODE:
```
dotnet user-secrets set "BedrockAgent:FoundationModel" "anthropic.claude-3-sonnet-20240229-v1:0"

```

----------------------------------------



TITLE: Account Creation Subprocess Flowchart
DESCRIPTION: Details the account creation subprocess, illustrating the flow from a new user form and form filling interaction to core system record creation. It shows subsequent steps like CRM and marketing record creation, leading to a welcome packet and final account creation success.

SOURCE: https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/GettingStartedWithProcesses/README.md#_snippet_5

LANGUAGE: mermaid
CODE:
```
graph LR
    NewUserForm([New User Form])
    NewUserFormConv([Form Filling Interaction])
    
    subgraph AccountCreation[Account Creation Process]
        direction LR
        AccountValidation([Account Verification Passed])
        NewUser1([New User Form])
        NewUserFormConv1([Form Filling Interaction])

        CoreSystem(Core System <br/> Record <br/> Creation)
        Marketing(New Marketing <br/> Record Creation) 
        CRM(CRM Record <br/> Creation)
        Welcome(Welcome <br/> Packet)
        NewAccountCreation([New Account Success])

        NewUser1-->CoreSystem
        NewUserFormConv1-->CoreSystem

        AccountValidation-->CoreSystem
        CoreSystem-->CRM-->|Success|Welcome
        CoreSystem-->Marketing-->|Success|Welcome
        CoreSystem-->|Account Details|Welcome

        Welcome-->NewAccountCreation
    end

    AccountVerificationPass-->AccountValidation
    NewUserForm-->NewUser1
    NewUserForm-->NewUser2
    NewUserFormConv-->NewUserFormConv1
```