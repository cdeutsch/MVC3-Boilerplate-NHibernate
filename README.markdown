# ASP.NET MVC3 Boilerplate - NHibernate Edition #

A collection of addons and configurations I use in most projects. Inspired by HTML5 Boilerplate.

I'd love to see someone smarter then me take this over and make it awesome. I'm sure there are things I have in here that may not be best practices.

This is a fork of the regular ASP.NET MVC3 Boilerplate project that uses Entity Framework:
https://github.com/crdeutsch/MVC3-Boilerplate

## Features ##

* HTML5 Boilerplate - http://html5boilerplate.com/
* Elmah - http://code.google.com/p/elmah/
* JSON Parser - https://github.com/douglascrockford/JSON-js
* Modernizr - http://www.modernizr.com/
* Ninject - http://ninject.org/
* Telerik MVC Extensions - http://www.telerik.com/products/aspnet-mvc.aspx
* Sql Server CE - http://nuget.org/Packages/Packages/Details/SqlServerCompact-4-0-8482-1
* NHibernate - http://www.nhforge.org/
* Fluent NHibernate - http://fluentnhibernate.org/
* NHibernate Validator - http://nhforge.org/wikis/validator/nhibernate-validator-1-0-0-documentation.aspx
* Bits from Tekpub MVC 2 Starter Site - http://mvcstarter.codeplex.com/
* Basic User Signup using simple POCO User object
* Cache Extensions - http://stackoverflow.com/questions/445050/how-can-i-cache-objects-in-asp-net-mvc
* Phil Haack's Enumeration Extensions - http://haacked.com/archive/2010/06/10/checking-for-empty-enumerations.aspx
* Rob Connery's Sugar Extensions - https://github.com/robconery/sugar

## History ##

### 7/31/2011 ###
* Updated Flash Message helpers to work without javascript.
* Added Z-index to flash message.

### 7/05/2011 ###
* Enabled Elmah error filtering by default.

### 6/19/2011 ###
* Added null check to CacheHelper.
* Added NHibernate CustomForeignKeyConvention

### 6/1/2011 ###
* Modified MVC Boilerplate to use NHibernate instead of Entity Framework


## Road Map ##

* Add latest jQuery once this Validation issue is fixed: http://bassistance.de/jquery-plugins/jquery-plugin-validation/
* Add Telerik MVC Controls - http://www.telerik.com/products/aspnet-mvc.aspx
* Add examples of using JSON Parser
* Create Nuget package?