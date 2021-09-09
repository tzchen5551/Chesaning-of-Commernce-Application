# Database Connection

## Using the database

### Getting a context

The easiest way to use the database is to use context managers. This ensures that
all references to the database are closed automatically when it is finished being used.

```cs
// With using statements at top of file
// Acts as the import for the module that
// holds the database context classes
using DirectorPortalDatabase;

// Where database needs to be used
using(var context = new DatabaseContext())
{
    // do things with database
}
```

### Getting data

EntityFramework uses LINQ for all the operations to the database. This provides
a simple to use way of querying data from the database.

#### Examples

##### Get business that starts with the letter B

```cs
List<Business> businesses = context.Businesses.Where(
        b => b.GStrBusinessName.ToLower().StartsWith("b")
    ).ToList();
```

##### Get all todo items

```cs
List<Todo> todos = context.TodoListItems.ToList();
```

##### Get all incomplete todo items

```cs
List<Todo> todos = context.TodoListItems.Where(todo => !todo.GBlnMarkedAsDone).ToList();
```

##### Get business representatives that represent multiple businesses

```cs
// This will loop through the business reps, and then the contact people
// (Many-To-Many) and find instances where a business rep is listed
// as the contact person multiple times.

// It's possible that this is easier with joins, but entityframework
// join statements are just weird
List<ContactPerson> reps = context.ContactPeople.Where(
        cp => context.BusinessReps.Where(
            br => br.GIntContactPersonId == cp.GIntId
        ).Count() > 1
    ).ToList();
```

### Adding new data

To add new data to the database, all that needs to be done is to create
a new object of the type you want to add to the database. For example, if you want to create a new business, you would just do

```cs
Business b = new Business() {
    GStrBusinessName = "Business Name";
    GIntYearEstablished = 2021;
    GEnumMembershipLevel = MembershipLevel.GOLD;
    // etc
};
context.Businesses.Add(b);
context.SaveChanges();
```

### Updating records

Updating records just requires you to pull the old record, change the values in it, and then save the changes on the context.

```cs
// Gets business ID 1
Business b = context.Businesses.FirstOrDefault(x => x.GIntId == 1);
b.GStrBusinessName = "New Business Name";
context.SaveChanges();
```

---

## Creating/Updating the database

Open the package manager console, if not already open

```
Tools > NuGet Package Manager > Package Manager Console
```

##### Package Manager Console

```powershell
Update-Database
```

## Creating a migration

Anytime the schema of the database is changed, a migration will need to be created.

This will automatically create all the table changes for the database, so no manual
work will need to be done to edit tables/columns.

After all the schema changes are made, run

##### Package Manager Console

```powershell
Add-Migration MigrationName
Update-Database
```
