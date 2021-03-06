﻿(function () {
    $(function () {
        initialiseClndr();
        $(document).on('click', '.js-clndrNextMonth', clndrNextMonth);
        $(document).on('click', '.js-clndrPreviousMonth', clndrPreviousMonth);
        $(document).on('change', '.js-filter-location', updateFilters);
        $(document).on('change', '.js-filter-interests', updateFilters);
        $(document).on('click', '.calendar-event', clickLoadEvent);
        $(document).on('click', '.js-event__close', clickCloseEvent);

        window.onpopstate = function (e) {
            if (e.state) {
                var path = e.state.path;
                if (path.indexOf('event') > 0) {
                    var id = path.substr(path.indexOf('event/') + 6, path.length);
                    loadEvent(id);
                } else {
                    closeEvent();
                }
            }
        };
    });

    var clndr;
    var interests;
    var locations;
    var clndrData;
    var unfilteredEvents;
    var scrollTop;

    var initialiseClndr = function () {
        interests = [];
        locations = [];
        unfilteredEvents = [];
        var from = moment().date(1).subtract(2, 'M').format('DD/MM/YYYY');
        var to = moment().date(1).add(2, 'M').format('DD/MM/YYYY');
        $('#calendar-month').text(moment().format('MMMM'));
        renderClndr([], moment());
        loadEvents(from, to);
        updateLocation('YorkshireDigital', '/');
    };
    var loadEvents = function(from, to) {
        //Add to event service
        $.ajax({
                url: "/events",
                type: "GET",
                data: {
                    from: from,
                    to: to
                }
            })
            .done(function(events) {
                var newEvents = addNewEvents(events);
                if (newEvents.length > 0) {
                    populateClndr(newEvents);
                }
            });
    };
    var render = function (data) {
        clndrData = data;
        var template = _.template($('#template-calendar').html());
        return template(data);
    };
    var softRender = function () {
        var template = _.template($('#template-calendar').html());
        $('.clndr').html(template(clndrData));
    }
    var renderClndr = function (month) {
        clndr = $('.events-calendar').clndr({
            template: $('#template-calendar').html(),
            forceSixRows: true,
            events: [],
            multiDayEvents: {
                startDate: 'start',
                endDate: 'end'
            },
            startWithMonth: month,
            daysOfTheWeek: [
                '<span class="header-day__sm">S</span><span class="header-day__md">Sun</span><span class="header-day__lg">Sunday</span>',
                '<span class="header-day__sm">M</span><span class="header-day__md">Mon</span><span class="header-day__lg">Monday</span>',
                '<span class="header-day__sm">T</span><span class="header-day__md">Tue</span><span class="header-day__lg">Tuesday</span>',
                '<span class="header-day__sm">W</span><span class="header-day__md">Wed</span><span class="header-day__lg">Wednesday</span>',
                '<span class="header-day__sm">T</span><span class="header-day__md">Thu</span><span class="header-day__lg">Thursday</span>',
                '<span class="header-day__sm">F</span><span class="header-day__md">Fri</span><span class="header-day__lg">Friday</span>',
                '<span class="header-day__sm">S</span><span class="header-day__md">Sat</span><span class="header-day__lg">Saturday</span>'
            ],
            render: render
        });
    };
    var populateClndr = function (events) {
        clndr.addEvents(events);
        updateMonthLabels();
        hideLoading();
    };
    var updateMonthLabels = function ()
    {
        var month = clndr.month;
        var eventsThisMonth = _.filter(clndr.eventsThisMonth, function (event) {
            return filterEventsByMonth(event, month);
        });
        $('#event-count').text(eventsThisMonth.length);
        $('#calendar-month').text(month.format('MMMM'));
    }
    var showLoading = function () {
        $('.loading-item__overlay').removeClass('is-hidden');
        $('.clndr-grid').addClass('loading__item');
    };
    var hideLoading = function () {
        $('.loading-item__overlay').addClass('is-hidden');
        $('.clndr-grid').removeClass('loading__item');
    };
    var addNewEvents = function (events) {
        var newEvents = _.filter(events, function (newEvent) {
            return _.every(unfilteredEvents, function (evt) {
                return evt.uniqueName !== newEvent.uniqueName;
            });
        });

        unfilteredEvents = _.union(unfilteredEvents, newEvents);
        populateFilters(newEvents);
        return newEvents;
    };
    var filterEventsByMonth = function (event, currentMonth) {
        return currentMonth.isSame(event.start, 'month');
    };
    var clndrNextMonth = function () {
        clndr.forward();
        updateMonthLabels();
        var from = moment(clndr.month._d).date(1).add(1, 'M').format('DD/MM/YYYY');
        var to = moment(clndr.month._d).date(1).add(2, 'M').format('DD/MM/YYYY');
        loadEvents(from, to);
    };
    var clndrPreviousMonth = function () {
        clndr.back();
        updateMonthLabels();
        var from = moment(clndr.month._d).date(1).subtract(2, 'M').format('DD/MM/YYYY');
        var to = moment(clndr.month._d).date(1).subtract(1, 'M').format('DD/MM/YYYY');
        loadEvents(from, to);
    };
    var populateFilters = function (events) {
        _.each(events, function (e) {
            interests = _.union(interests, e.interests);
            if (e.region) {
                locations = _.union(locations, e.region.split('|'));
            }
        });

        interests = interests.sort();
        locations = locations.sort();

        _.each(interests, function (interest) {
            if ($('option[value="' + interest + '"]', '.js-filter-interests').length === 0) {
                $('.js-filter-interests').append($('<option/>', {
                    value: interest,
                    text: interest
                }));
            }
        });

        _.each(locations, function (location) {
            if ($('option[value="' + location + '"]', '.js-filter-location').length === 0) {
                $('.js-filter-location').append($('<option/>', {
                    value: location,
                    text: location
                }));
            }
        });
    };
    var updateFilters = function () {
        softRender();
    };
    var clickLoadEvent = function (event) {
        scrollTop = $(window).scrollTop();
        event.preventDefault();
        var id = $(this).data('id');
        var title = $('.calendar-event__title', this).text();
        loadEvent(id, title, true);
    }
    var loadEvent = function (id, title) {
        if ($('#' + id).length > 0) {
            $('#' + id).removeClass('is-hidden');
            $('body').addClass('no-scroll');
            if (title) {
                updateLocation(title + ' : YorkshireDigital', '/event/' + id);
            }
        } else {
            $.ajax({
                url: '/event/' + id,
                type: 'GET',
                headers: {
                    Accept: "text/html; charset=utf-8"
                },
                data: { partial: true }
            })
            .done(function (html) {
                updateLocation(title + ' : YorkshireDigital', '/event/' + id);

                $('.page-content').after(html);
                $('body').addClass('no-scroll');
            });
        }
    };
    var clickCloseEvent = function() {
        closeEvent(true);
    }
    var closeEvent = function (updateLoc) {
        event.preventDefault();
        $('body').removeClass('no-scroll');
        $('.modal').addClass('is-hidden');

        if (scrollTop) {
            $(window).scrollTop(scrollTop);
            scrollTop = null;
        };

        if (updateLoc) {
            updateLocation('YorkshireDigital', '/');
        }
    };
    var updateLocation = function(pageTitle, path) {
        document.title = pageTitle;
        window.history.pushState({ "html": document.documentElement.innerHTML, "pageTitle": pageTitle, "path": path }, "", path);
        ga('send', 'pageview');
    };
}());