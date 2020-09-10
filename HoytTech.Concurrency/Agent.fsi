module HoytTech.Concurrency.Agent

open System.Threading.Tasks

module AgentFunctor =
    
    /// The type of the result from the agent.
    type result<'t> =
        | Full
        | Success of 't
    
    /// The type for the action execute.
    type action<'a> = 'a -> 'a
    
    /// The result type.
    type result<'a, 'r> = 'a -> 'r
    
    /// The type to return.
    type t<'id, 'a>
    
    /// Used to make a new agent.
    /// <param name="size">The size of the queue to user.</param>
    /// <param name="id">The id of the agent.</param>
    /// <param name="value">The value to protect with the agent.</param>
    val make<'id, 'a> : int32 -> 'id -> 'a -> t<'id, 'a>
    
    /// Applies a stateful fmap change.
    /// <param name="acton">The action to execute.  Returns the new value.</param>
    /// <returns>The task that executes the action.</returns>
    val fmap<'id, 'a> : t<'id, 'a> ->  action<'a> -> Task<t<'id, 'a> * result<'a>>
    
    /// Applies a map on the value.
    /// <param name="func">The mapping function to call.</param>
    /// <returns>The task with the result.</returns>
    val map<'id, 'a, 'r> : t<'id, 'a> -> result<'a, 'r> -> Task<result<'r>>
    
