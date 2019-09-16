﻿// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace TheSummit

open System.Diagnostics
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module App =
    type Model =
        { Count : int
          Step : int
          TimerOn : bool }

    type Msg =
        | Increment
        | Decrement
        | Reset
        | SetStep of int
        | TimerToggled of bool
        | TimedTick

    let initModel =
        { Count = 0
          Step = 1
          TimerOn = false }

    let init() = initModel, Cmd.none

    let timerCmd =
        async {
            do! Async.Sleep 200
            return TimedTick
        }
        |> Cmd.ofAsyncMsg

    let update msg model =
        match msg with
        | Increment -> { model with Count = model.Count + model.Step }, Cmd.none
        | Decrement -> { model with Count = model.Count - model.Step }, Cmd.none
        | Reset -> init()
        | SetStep n -> { model with Step = n }, Cmd.none
        | TimerToggled on ->
            { model with TimerOn = on },
            (if on then timerCmd
             else Cmd.none)
        | TimedTick ->
            if model.TimerOn then { model with Count = model.Count + model.Step }, timerCmd
            else model, Cmd.none

    let view (model : Model) dispatch =
        View.ContentPage
            (content = View.StackLayout
                           (padding = 20.0, verticalOptions = LayoutOptions.Center,
                            children = [ View.Label
                                             (text = sprintf "%d" model.Count, horizontalOptions = LayoutOptions.Center,
                                              widthRequest = 200.0, horizontalTextAlignment = TextAlignment.Center)
                                         View.Button
                                             (text = "Increment", command = (fun () -> dispatch Increment),
                                              horizontalOptions = LayoutOptions.Center, backgroundColor = Color.Red)
                                         View.Button
                                             (text = "Decrement", command = (fun () -> dispatch Decrement),
                                              horizontalOptions = LayoutOptions.Center)
                                         View.Label(text = "Timer", horizontalOptions = LayoutOptions.Center)
                                         View.Switch
                                             (isToggled = model.TimerOn,
                                              toggled = (fun on -> dispatch (TimerToggled on.Value)),
                                              horizontalOptions = LayoutOptions.Center)
                                         View.Slider
                                             (minimumMaximum = (0.0, 10.0), value = double model.Step,
                                              valueChanged = (fun args -> dispatch (SetStep(int (args.NewValue + 0.5)))),
                                              horizontalOptions = LayoutOptions.FillAndExpand)
                                         View.Label
                                             (text = sprintf "Step size: %d" model.Step,
                                              horizontalOptions = LayoutOptions.Center)
                                         View.Button
                                             (text = "Reset", horizontalOptions = LayoutOptions.Center,
                                              command = (fun () -> dispatch Reset), canExecute = (model <> initModel)) ]))

    // Note, this declaration is needed if you enable LiveUpdate
    let program = Program.mkProgram init update view

type App() as app =
    inherit Application()
    let runner =
        App.program
        |> Program.withConsoleTrace
        |> XamarinFormsProgram.run app
// Uncomment this line to enable live update in debug mode.
// See https://fsprojects.github.io/Fabulous/tools.html for further  instructions.
//
//do runner.EnableLiveUpdate()
// Uncomment this code to save the application state to app.Properties using Newtonsoft.Json
// See https://fsprojects.github.io/Fabulous/models.html for further  instructions.
