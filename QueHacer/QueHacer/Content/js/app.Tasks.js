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
    t.push("CategoryList");

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
    var TaskContainer = $("#TasksContainer");
    var CategoryList = $("#CategoryList");
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

        RenderFilterList();
    }
    function RenderFilterList(callback) {
        app.Util.CompileTemplates(templatesURL, t, tasks, function () {
            if (todoDB.Tasks.length == 0) {
                CategoryList.empty().append("No filters");
            } else {
                var cats = [];
                for (var i = 0, l = todoDB.Tasks.length; i < l; i++) {
                    if(!$.trim(todoDB.Tasks[i].category) == "")
                        cats.push(todoDB.Tasks[i].category);
                }

                var data = _.uniq(cats);
                data.splice(0, 0, "all");
                if (data.length == 0)
                    CategoryList.empty().append("No filters");
                else
                    CategoryList.empty().append(tasks.tl.CategoryList(data.sort()));
            }

            if (callback)
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
            var d;
            if (filter.category == "all")
                d = _.filter(todoDB.Tasks, function (t) { return t.status == tasksList.status.unfinished });
            else
                d = _.filter(todoDB.Tasks, function (t) { return t.status == tasksList.status.unfinished && t.category == filter.category });

            return _.sortBy(d, function (t) { return t.duedate }); //.reverse();
        }
        
        // only filter by status
        if (filter.status && !filter.dateFilter) {
            switch (filter.status) {
                case tasksList.status.complete:
                    if (filter.category == "all")
                        return _.filter(todoDB.Tasks, function (t) { return t.status == tasksList.status.complete });
                    else
                        return _.filter(todoDB.Tasks, function (t) { return t.status == tasksList.status.complete && t.category == filter.category });

                    break;
                case tasksList.status.overdue:
                    var today = new Date();
                    dateFilter = Date.parse(new Date((today.getMonth() + 1) + "/" + today.getDay() + "/" + today.getFullYear()).toUTCString())

                    if (filter.category == "all")
                        return _.filter(todoDB.Tasks, function (t) { return t.duedate != 0 && t.duedate < dateFilter });
                    else
                        return _.filter(todoDB.Tasks, function (t) { return t.duedate != 0 && t.duedate < dateFilter && t.category == filter.category });
                    break;
            }
            
        }

        // only filter by dateFilter
        if (!filter.status && filter.dateFilter) {
            var dateFilter;
            switch (filter.dateFilter) {
                case tasksList.dateFilter.Today:
                    var today = new Date();
                    dateFilter = Date.parse(new Date((today.getMonth() + 1) + "/" + today.getDay() + "/" + today.getFullYear()).toUTCString())
                    
                    if(filter.category == "all")
                        return _.filter(todoDB.Tasks, function (t) { return t.duedate == dateFilter });
                    else
                        return _.filter(todoDB.Tasks, function (t) { return t.duedate == dateFilter && t.category == filter.category });

                    break;
                case tasksList.dateFilter.Tomorrow:
                    // to add 1 day
                    var today = new Date();
                    var almostTomorrow = new Date((today.getMonth() + 1) + "/" + today.getDay() + "/" + today.getFullYear());
                    almostTomorrow.setDate(today.getDate() + 1);
                    dateFilter = app.Util.DatetoUTC(almostTomorrow);

                    if (filter.category == "all")
                        return _.filter(todoDB.Tasks, function (t) { return t.duedate == dateFilter });
                    else
                        return _.filter(todoDB.Tasks, function (t) { return t.duedate == dateFilter && t.category == filter.category });

                    break;
                case tasksList.dateFilter.ThisWeek:
                    // to add 7 day
                    var today = new Date();
                    var todayUTC = Date.parse(new Date((today.getMonth() + 1) + "/" + today.getDay() + "/" + today.getFullYear()).toUTCString())

                    var onlydateNoTime = new Date((today.getMonth() + 1) + "/" + today.getDay() + "/" + today.getFullYear());
                    onlydateNoTime.setDate(today.getDate() + 7);
                    var SevendaysFromNow = app.Util.DatetoUTC(onlydateNoTime);

                    var d;
                    if (filter.category == "all")
                        d = _.filter(todoDB.Tasks, function (t) { return t.duedate >= todayUTC && t.duedate <= SevendaysFromNow });
                    else
                        d = _.filter(todoDB.Tasks, function (t) { return t.duedate >= todayUTC && t.duedate <= SevendaysFromNow && t.category == filter.category });

                    return _.sortBy(d, function (t) { return t.duedate }); //.reverse();
                    break;
                case tasksList.dateFilter.NextWeek:
                    var today = new Date();
                    var onlydateNoTime = new Date((today.getMonth() + 1) + "/" + today.getDay() + "/" + today.getFullYear());
                    onlydateNoTime.setDate(today.getDate() + 8);
                    var EightdaysFromNow = app.Util.DatetoUTC(onlydateNoTime);

                    onlydateNoTime.setDate(onlydateNoTime.getDate() + 7);
                    var EndofNextWeek = app.Util.DatetoUTC(onlydateNoTime);

                    var d;
                    if (filter.category == "all")
                        d = _.filter(todoDB.Tasks, function (t) { return t.duedate >= EightdaysFromNow && t.duedate <= EndofNextWeek });
                    else
                        d = _.filter(todoDB.Tasks, function (t) { return t.duedate >= EightdaysFromNow && t.duedate <= EndofNextWeek && t.category == filter.category });

                    return _.sortBy(d, function (t) { return t.duedate });
                    break;
                case tasksList.dateFilter.ThisMonth:
                    var today = new Date();
                    var todayUTC = Date.parse(new Date((today.getMonth() + 1) + "/" + today.getDay() + "/" + today.getFullYear()).toUTCString())

                    var onlydateNoTime = new Date((today.getMonth() + 1) + "/" + today.getDay() + "/" + today.getFullYear());
                    onlydateNoTime.setDate(today.getDate() + 30);
                    var OneMonthFromNow = app.Util.DatetoUTC(onlydateNoTime);

                    var d;
                    if (filter.category == "all")
                        d = _.filter(todoDB.Tasks, function (t) { return t.duedate >= todayUTC && t.duedate <= OneMonthFromNow });
                    else
                        d = _.filter(todoDB.Tasks, function (t) { return t.duedate >= todayUTC && t.duedate <= OneMonthFromNow && t.category == filter.category });

                    return _.sortBy(d, function (t) { return t.duedate });
                    break;
            }

            
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
        render: function (callback) {
            RenderTasks(callback);
        },
        status: {
            complete: "completed",
            unfinished: "unfinished",
            overdue: "overdue"
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