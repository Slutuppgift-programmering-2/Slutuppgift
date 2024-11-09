
---
## ViewModel

GraphViewModel and RouteViewModel has to be synchronized in that way any changes on the Routes should be displayed on the Graph.

In order to keep both ViewModels synchronized will be done by using a **share data source** in a central MainViewModel. This approach allows each ViewModel to manage its own view-specific logic while still being notified of changes in the shared data.

Solution: Use `MainViewModel` as a shared data source with Change Notification.

- **Shared Data Collection** in `MainViewModel`
	Store the central data in `MainViewModel` as `ObservableCollection<Route> Routes` and `ObservableCollection<CityNode> Cities`.`
	objects so changes are automatically propagated to the ViewModels.
- **Use `INotifyPropertyChanged`**
	By exposing collections as `ObsevableCollection` and implementing `INotifyPropertyChanged`, you can keep track of changes and notify dependent ViewModels of changes to the shared data.`
- **Bind both ViewModels to `MainViewModel`**
	Both `GraphViewModel` and `RouteViewModel` will bind to the shared data in `MainViewModel` to keep the views synchronized.`

### `RouteViewModel` and `GraphViewModel` Interaction` 
Now, both `RouteViewModel` and `GraphViewModel` will be constructed with a reference to `MainViewModel`. This way, they can observe changes in the shared data.

- **`RouteViewModel`**
	- `RouteViewModel` will bind to the `Routes` collection in `MainViewModel` to display the list of routes.
	- `RouteViewModel` will also have a method to add a new route to the `Routes` collection in `MainViewModel`.
- **`GraphViewModel`**
	- `GraphViewModel` will bind to the `Cities` collection in `MainViewModel` to display the graph.
	- `GraphViewModel` will also have a method to add a new city to the `Cities` collection in `MainViewModel`.

### Initialize and Bind ViewModels in `MainViewModel`
In `MainViewModel`, you will initialize both `RouteViewModel` and `GraphViewModel` and bind them to the shared data.

## **Summary**
- **Centralized Data**: `MainViewModel` holds the shared data (`Cities` and `Routes`).
- **Separate ViewModels**: `RouteViewModel` and `GraphViewModel` access the shared data by referencing `MainViewModel`.
- **Data Synchronization**: Updates in `MainViewModel` collections are automatically reflected in both views due to `ObservableCollection` and change notification mechanisms.
- **Shared Data Collection** in MainViewModel
- **Use INotifyPropertyChanged**
- **Bind both ViewModels to MainViewModel**
- **RouteViewModel and GraphViewModel Interaction**
- **Initialize and Bind ViewModels in MainViewModel**
