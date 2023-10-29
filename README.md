# Cogniva.Configuration

Extracting and refining some code Cogniva uses to manage configuration information

## Motivation

At Cogniva, we regularly need the ability to extensively customise the behaviour of our software at runtime - turning certain processing on or off, providing connection strings to different systems, and so on. Keeping track of all the options, their possible values, and so on, became a significant challenge, and led us to create some tools that can use attributes in the source code to both control how a key is used and to provide information that can be used to generate documentation.

We've benefitted a great deal from open-source software provided by others, and we think that the tools we've built to make config management easier for ourselves could also be of value to others, so we've extracted the core aspects of that code and made it into an open-source project.

## What can I do with this library?

We use it internally to do a number of things. Here are the current cases:

- Provide default values for configuration keys
- Control where we load configuration key values from (e.g. whether to read it from INI files)
- Generate documentation at compile time
- Provide detailed help information to the user at runtime (e.g. letting the user know that a particular key expects an integer value, listing the default, and so on)
- Create user interfaces that validate the values provided by the user to ensure that they'll work for a given configuration key

This library isn't going to be useful for everyone - if you only need a few different configurable elements, managing them by hand is likely to be more than sufficient. But with the sheer volume of different ways we might need to adjust the behaviour of our software at runtime, having the ability for these things to be self-describing allows us to generate documentation and simplify the UI, saving us a lot of time and hassle.

## More than just config keys

The first couple iterations of this library were built specifically for configuration keys, and really helped. With the latest version though, we're expanding the utility quite a bit. Here are a couple things we're working on internally (at least some of which will appear in this public library):

### Connection strings

We connect to a lot of different systems, each of which need different types of information. For example, if we want to read information from a SharePoint library, we need the site's URL, we need the library name, we need to know what sort of authentication to use, etc.

By creating appropriate connection classes in C# and adding appropriate attributes, we'll be able to automatically extract information about how we connect to SharePoint - the names of the keys to use, descriptions of what they do, default values that will be used if none is provided, information about which ones are mandatory, and so on. From there we'll be able to generate documentation, provide feedback to the user at runtime, and even generate UIs automatically to make it even easier to enter the required configuration info.

### Workflow definition (e.g. autoclassification pipelines)

Cogniva uses sophisticated AI algorithms to allow customers to automatically classify documents. We have a number of standard algorithms we can use that provide excellent performance, but in some cases we need more ability to customise the behaviour of the algorithms to optimise accuracy or performance. This type of setup is accomplished using what is essentially a workflow definition, and customising the setup beyond the standard (generally very effective) configuration requires a lot of detailed knowledge.

We're working on ways to describe each of the pipeline elements using this library. As with the other cases, this will let us generate documentation automatically, provide helpful feedback to users at runtime, and even generate UIs automatically to make it even easier to set these up. 

## More to come...

While we have a lot of the code already written, bringing it into this library, getting builds set up, and so on will take some time. We're then going to be working on an updated version of the library that'll simplify using this library by taking advantage of an incremental source generator.

If you're interested in contributing, please leave us a message in the Discussion section to talk about it, or contact Christine Salter!