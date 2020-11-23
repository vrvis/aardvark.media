﻿namespace Aardvark.UI.Anewmation

open Aardvark.Base
open FSharp.Data.Adaptive

[<AutoOpen>]
module AnimatorQueryingAdaptive =

    module Animator =

        open InternalAnimatorUtilities.Converters

        /// Tries to get the (untyped) animation with the given name.
        /// The name can be a string or Symbol. Returns None if the
        /// animation does not exist.
        let inline tryGetUntyped' (name : ^Name) (animator : AdaptiveAnimator<'Model, _, _>) =
            let sym = name |> symbol Unchecked.defaultof<NameConverter>
            animator.Animations |> AMap.tryFind sym

        /// <summary>
        /// Gets the (untyped) animation with the given name.
        /// The name can be a string or Symbol.
        /// </summary>
        /// <exception cref="KeyNotFoundException">Thrown if the animation does not exist.</exception>
        let inline getUntyped' (name : ^Name) (animator : AdaptiveAnimator<'Model, _, _>) =
            let sym = name |> symbol Unchecked.defaultof<NameConverter>
            animator.Animations |> AMap.find sym

        /// Tries to get the animation with the given name.
        /// The name can be a string or Symbol. Returns None if
        /// the animation does not exist or is not of the expected type.
        let inline tryGet' (name : ^Name) (animator : AdaptiveAnimator<'Model, _, _>) : aval<IAnimation<'Model, 'Value> option> =
            animator |> tryGetUntyped' name |> AVal.map (Option.bind (function
                | :? IAnimation<'Model, 'Value> as a -> Some a
                | _ -> None
            ))


        /// <summary>
        /// Gets the animation with the given name.
        /// The name can be a string or Symbol.
        /// </summary>
        /// <exception cref="KeyNotFoundException">Thrown if the animation does not exist.</exception>
        /// <exception cref="InvalidCastException">Thrown if the animation is not of the expected type.</exception>
        let inline get' (name : ^Name) (animator : AdaptiveAnimator<'Model, _, _>) : aval<IAnimation<'Model, 'Value>> =
            animator |> getUntyped' name |> AVal.map unbox

        /// Returns whether an animation with the given name exists.
        /// The name can be a string or Symbol.
        let inline exists' (name : ^Name) (animator : AdaptiveAnimator<'Model, _, _>) =
            let sym = name |> symbol Unchecked.defaultof<NameConverter>
            animator.Animations |> AMap.exists (fun k _ -> k = sym)

        /// Tries to get the state of the animation with the given name.
        /// The name can be a string or Symbol. Returns None if the
        /// animation does not exist.
        let inline tryGetState' (name : ^Name) (animator : AdaptiveAnimator<'Model, _, _>) =
            animator |> tryGetUntyped' name |> AVal.map (Option.map (fun a -> a.State))

        /// <summary>
        /// Gets the state of the animation with the given name.
        /// The name can be a string or Symbol.
        /// </summary>
        /// <exception cref="KeyNotFoundException">Thrown if the animation does not exist.</exception>
        let inline getState' (name : ^Name) (animator : AdaptiveAnimator<'Model, _, _>) =
            animator |> getUntyped' name |> AVal.map (fun a -> a.State)

        /// Returns whether the animation with the given name is running.
        /// The name can be a string or Symbol. Returns false if the
        /// animation does not exist.
        let inline isRunning' (name : ^Name) (animator : AdaptiveAnimator<'Model, _, _>) =
            animator |> tryGetUntyped' name |> AVal.map (Option.map (fun a -> a.IsRunning) >> Option.defaultValue false)

        /// Returns whether the animation with the given name is stopped.
        /// The name can be a string or Symbol. Returns true if the
        /// animation does not exist.
        let inline isStopped' (name : ^Name) (animator : AdaptiveAnimator<'Model, _, _>) =
            animator |> tryGetUntyped' name |> AVal.map (Option.map (fun a -> a.IsStopped) >> Option.defaultValue true)

        /// Returns whether the animation with the given name is finished.
        /// The name can be a string or Symbol. Returns true if the
        /// animation does not exist.
        let inline isFinished' (name : ^Name) (animator : AdaptiveAnimator<'Model, _, _>) =
            animator |> tryGetUntyped' name |> AVal.map (Option.map (fun a -> a.IsFinished) >> Option.defaultValue true)

        /// Returns whether the animation with the given name is paused.
        /// The name can be a string or Symbol. Returns false if the
        /// animation does not exist.
        let inline isPaused' (name : ^Name) (animator : AdaptiveAnimator<'Model, _, _>) =
            animator |> tryGetUntyped' name |> AVal.map (Option.map (fun a -> a.IsPaused) >> Option.defaultValue false)