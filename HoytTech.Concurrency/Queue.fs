module HoytTech.Concurrency.Queue

open System.Runtime.InteropServices
open HoytTech.Concurrency.Padding

module Mpmc =
    
    type t<'a> = {
        buffer: ConcurrentRingBuffer.t<'a>
        padding: Padding.PaddedLong.t
        producerIndex: Padding.PaddedLong.t
        consumerIndex: Padding.PaddedLong.t
    }
    
    let make pos =
        {
            buffer = ConcurrentRingBuffer.make pos
            padding = PaddedLong.make 0L
            producerIndex = PaddedLong.make 0L
            consumerIndex = PaddedLong.make 0L
        }
    
    let procedureIndex t =
        PaddedLong.read t.producerIndex
        
    let consumerIndex t =
        PaddedLong.read t.consumerIndex
        
    let offer t value =
        let capacity = ConcurrentRingBuffer.length t.buffer
        let rec addLoop i =
            let pIndex = PaddedLong.read t.producerIndex
            let cIndex = PaddedLong.read t.consumerIndex
            if pIndex - capacity >= cIndex then
                None
            else if ConcurrentRingBuffer.hasValue t.buffer pIndex then
                addLoop (i + 1)
            else if PaddedLong.compareExchange t.producerIndex (pIndex + 1L) pIndex then
                Some(pIndex)
            else
                addLoop (i + 1)
        let pIndex = addLoop 0
        match pIndex with
        | Some(pIndex) ->
            ConcurrentRingBuffer.set t.buffer pIndex value
        | None -> false
        
    let poll t =
        let rec getLoop i =
            let pIndex = PaddedLong.read t.producerIndex
            let cIndex = PaddedLong.read t.consumerIndex
            if cIndex >= pIndex then
                None
            else if ConcurrentRingBuffer.empty t.buffer cIndex then
                getLoop i
            else if PaddedLong.compareExchange t.consumerIndex (cIndex + 1L) cIndex then
                Some(cIndex)
            else
                getLoop (i + 1)
        let cIndex = getLoop 0
        match cIndex with
        | Some(cIndex) ->
            let value = ConcurrentRingBuffer.get t.buffer cIndex
            ConcurrentRingBuffer.clear t.buffer cIndex |> ignore
            value
        | None -> None
        
    let peek t =
        let cIndex = PaddedLong.read t.consumerIndex
        ConcurrentRingBuffer.get t.buffer cIndex
            
                
module SkipQueue =
    
    type whichQueue =
        | Primary
        | Secondary
    
    type t<'a> = {
        primary: Mpmc.t<'a>
        skip: Mpmc.t<'a>
        queue: whichQueue ref
    }
    
    let make queueSize skipSize =
        {
            primary = Mpmc.make queueSize
            skip = Mpmc.make skipSize
            queue = ref Primary 
        }
        
    let offer t value =
        Mpmc.offer t.primary value
        
    let poll t =
        match !t.queue with
        | Primary -> Mpmc.poll t.primary
        | Secondary ->
            match Mpmc.poll t.skip with
            | Some(a) -> Some(a)
            | None ->
                t.queue := Primary
                Mpmc.poll t.primary

    let defer t value =
        Mpmc.offer t.skip value
                
    let reset t =
        t.queue := Secondary
        