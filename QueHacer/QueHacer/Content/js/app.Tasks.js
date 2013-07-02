/// <reference path="util.js" />
/// <reference path="underscore-min.js" />

(function () {
    
    // globals
    var tasks = {};
    var templatesURL = "Content/tmpl/main.htm";
    var t = [];
    t.push("TasksList");
    t.push("Tasks");
    t.push("NoTasks");
    t.push("NoTasksCustom");

    var TaskContainer = $("#TasksContainer");

    // controller actions
    function AddTaskPOST(NewTask, callback) {
        var errorMsg = "Missing a parameter!";
        if (NewTask == null) { throw errorMsg; return; }
        if (callback == null) { throw errorMsg; return; }

        $.ajax({
            type: "POST",
            url: "/App/AddTask/",
            dataType: "json",
            data: JSON.stringify(NewTask),
            contentType: 'application/json; charset=utf-8',
            processData: false,
            cache: false,
            success: function (result) {
                callback(result);
            }
        });
    }
    function DeleteTaskPOST(id, callback) {
        var errorMsg = "Missing a parameter!";
        if (id == null) { throw errorMsg; return; }
        if (callback == null) { throw errorMsg; return; }

        var packet = {
            id: id
        };

        $.ajax({
            type: "POST",
            url: "/App/DeleteTask/",
            dataType: "json",
            data: JSON.stringify(packet),
            contentType: 'application/json; charset=utf-8',
            processData: false,
            cache: false,
            success: function (result) {
                callback(result);
            }
        });
    }
    function UpdateTaskPOST(Task, callback) {
        var errorMsg = "Missing a parameter!";
        if (Task == null) { throw errorMsg; return; }
        if (callback == null) { throw errorMsg; return; }

        $.ajax({
            type: "POST",
            url: "/App/UpdateTask/",
            dataType: "json",
            data: JSON.stringify(Task),
            contentType: 'application/json; charset=utf-8',
            processData: false,
            cache: false,
            success: function (result) {
                callback(result);
            }
        });
    }

    // render methods
    function RenderTasks(callback) {
        app.Util.CompileTemplates(templatesURL, t, tasks, function () {
            if (todoDB.Tasks.length == 0) {
                TaskContainer.empty().append(tasks.tl.NoTasks());
            } else {
                var data = GetData(tasksList.currentFilter);
                if (data.length == 0)
                    TaskContainer.empty().append(tasks.tl.NoTasksCustom());
                else
                    TaskContainer.empty().append(tasks.tl.TasksList({ rows: tasks.tl.Tasks(data) }));
            }

            if(callback)
                callback();
        });
    }

    /*
    filter = {
        status: [statuses from app.Tasks.status],
        dateFilter: [statuses from app.Tasks.dateFilter]
    }
    */
    function GetData(filter) {
        // no filter return all not completed
        if (filter.dateFilter == null && filter.status == null) {
            var d = _.filter(todoDB.Tasks, function (t) { return t.status == tasksList.status.unfinished });
            return _.sortBy(d, function (t) { return t.duedate }).reverse();
        }
        
        // only filter by status
        if (filter.status && !filter.dateFilter) {
            return _.filter(todoDB.Tasks, function (t) { return t.status == filter.status });
        }

        // only filter by dateFilter
        if (!filter.status && filter.dateFilter) {
            switch (filter.dateFilter) {
                case tasksList.dateFilter.Today:

            }

            return _.filter(todoDB.Tasks, function (t) { return t.dateFilter == filter.dateFilter });
        }
    }

    var tasksList = {
        add: function (item, callback) {
            AddTaskPOST(item, function (result) {
                if (!result.IsError) {
                    todoDB.Tasks.push(item);
                    RenderTasks();
                }
                callback(result);
            });
        },
        Delete: function (id, callback) {
            DeleteTaskPOST(id, function (result) {
                if (!result.IsError) {
                    for (var i = 0, l = todoDB.Tasks.length; i < l; i++) {
                        if (todoDB.Tasks[i]._id == id) {
                            todoDB.Tasks.splice(i, 1);

                            if (todoDB.Tasks.length == 0)
                                RenderTasks();

                            break;
                        }
                    }
                }
                callback(result);
            });
        },
        update: function(item, callback){
            UpdateTaskPOST(item, function (result) {
                if (!result.IsError) {
                    for (var i = 0, l = todoDB.Tasks.length; i < l; i++) {
                        if (todoDB.Tasks[i]._id == item._id) {
                            todoDB.Tasks[i] = item;
                            break;
                        }
                    }
                }
                callback(result);
            });
        },
        getTask: function(id){
            for (var i = 0, l = todoDB.Tasks.length; i < l; i++) {
                if (todoDB.Tasks[i]._id == id) {
                    return todoDB.Tasks[i];
                }
            }
        },
        filterByTime: function () {

        },
        filterByCategory: function () {

        },
        render: function (callback) {
            RenderTasks(callback);
        },
        status: {
            complete: "completed",
            unfinished: "unfinished"
        },
        dateFilter: {
            Today: "today",
            Tomorrow: "tomorrow",
            ThisWeek: "thisweek",
            NextWeek: "nextweek",
            ThisMonth: "thismonth"
        },
        currentFilter: null
    };

    if (!window.app)
        window.app = {};

    if (!window.app.Tasks)
        window.app.Tasks = tasksList;
})();