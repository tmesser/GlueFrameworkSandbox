This is a sample enterprise data access layer that I will demonstrate Glue integration with.
You will see some stuff that looks like it's connecting to a database (DbAddress, DbDate), 
and other stuff that looks like web services (the Requests/Responses folder).  This is intentional.
It shouldn't matter what Data Access is using, only that you have clearly thought out how its objects
relates to items in other layers.

This particular project follows the Onion Architecture, so while every project has visibility into levels
closer to the core, there is no visibility between layers on the same 'level'.  Therefore, the Service layer
will have no visibility to the Data Access layer, and vice versa.  The UI layer and other layers are not shown 
in this example, but after seeing how Data Access is to interact with the Domain/Service layers it should hopefully
be pretty clear how to continue this pattern.  The logic certainly does not change.