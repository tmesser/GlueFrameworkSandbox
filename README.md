GlueFrameworkSandbox
====================

A sandbox showing off the Glue framework by Tore Vestues

This is a sample enterprise data access/service/domain layer that I will demonstrate Glue integration with.
You will see some stuff that looks like it's connecting to a database (DbAddress, DbDate), 
and other stuff that looks like web services (the Requests/Responses folder).  This is intentional.
It shouldn't matter what Data Access is using, only that you have clearly thought out how its objects
relates to items in other layers.  Most of the interesting stuff happens in MappingFactory.cs in the DataAccess project.

The UI layer and other layers are not shown in this example, but after seeing how Data Access is to 
interact with the Domain/Service layers it should hopefully be pretty clear how to continue this pattern.
The logic certainly does not change.

TODOS:

-Finish the couple of unit tests that are not done in the Tests project

-Play around with the TypeSwitch class in Domain, to make it a little more handy to use

-Flesh out the sample code a bit more be a bit more illustrative of what's going on

-Make more mapping examples

-General cleanup of small mistakes that I'm sure I missed while making this.