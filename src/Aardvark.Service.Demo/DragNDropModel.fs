﻿namespace DragNDrop

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.UI

type Drag = { PickPoint : V3d; Offset : V3d }

[<DomainType>]
type Model = { 
    trafo       : Trafo3d 
    dragging    : Option<Drag>
    camera      : CameraControllerState
}

type Axis = X | Y | Z

type PickPoint =
    {
        offset : float
        axis   : Axis
    }


[<DomainType>]
type Transformation = { 
    trafo       : Trafo3d 
    hovered     : Option<Axis>
    grabbed     : Option<PickPoint>
}

[<DomainType>]
type Scene = {
    transformation : Transformation
    camera         : CameraControllerState
}