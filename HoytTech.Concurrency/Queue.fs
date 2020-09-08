module HoytTech.Concurrency.Queue

open HoytTech.Concurrency.Padding

module Mpsc =
    
    type t<'a> = {
        buffer: ConcurrentRingBuffer.t<'a>
        padding: Padding.PaddedLong.t
        producerIndex: Padding.PaddedLong.t
        consumerIndex: Padding.PaddedLong.t
    }
    
    let procedureIndex t =
        PaddedLong.read t.producerIndex
        
    let consumerIndex t =
        PaddedLong.read t.consumerIndex
        
    let offer t value =
        let capacity = ConcurrentRingBuffer.length t.buffer
        let rec addLoop _ =
            let pIndex = PaddedLong.read t.producerIndex
            let cIndex = PaddedLong.read t.consumerIndex
            if pIndex - capacity >= cIndex then
                None
            else if ConcurrentRingBuffer.hasValue t.buffer pIndex then
                addLoop ()
            else if PaddedLong.compareExchange t.producerIndex (pIndex + 1L) pIndex then
                Some(pIndex)
            else
                addLoop ()
        let pIndex = addLoop(())
        match pIndex with
        | Some(pIndex) ->
            ConcurrentRingBuffer.set t.buffer pIndex value
        | None -> false
        
    let poll t =
        let rec getLoop _ =
            let pIndex = PaddedLong.read t.producerIndex
            let cIndex = PaddedLong.read t.consumerIndex
            if cIndex >= pIndex then
                None
            else if ConcurrentRingBuffer.empty t.buffer cIndex then
                getLoop ()
            else if PaddedLong.compareExchange t.consumerIndex (cIndex + 1L) cIndex then
                Some(cIndex)
            else
                getLoop ()
        let cIndex = getLoop ()
        match cIndex with
        | Some(cIndex) ->
            let value = ConcurrentRingBuffer.get t.buffer cIndex
            ConcurrentRingBuffer.clear t.buffer cIndex |> ignore
            value
        | None -> None
        
    let peek t =
        let cIndex = PaddedLong.read t.consumerIndex
        ConcurrentRingBuffer.get t.buffer cIndex
            
            
                
            
