using System;

using System.Collections.Generic;



public class ConcurrentQueue<T>{
	
	private readonly object syncLock = new object();
	
	private Queue<T> queue;
	
	
	

	
	
	public int Count
		
	{
		
		get
			
		{
			
			lock(syncLock)
				
			{
				
				return queue.Count;
				
			}
			
		}
		
	}
	
	
	

	
	
	public ConcurrentQueue()
		
	{
		
		this.queue = new Queue<T>();
		
	}
	
	
	
	public void Enqueue(T item)
		
	{
		
		lock(syncLock)
			
		{
			
			queue.Enqueue(item);
			
		}
		
	}
	
	
	
	public T Dequeue()
		
	{
		
		lock(syncLock)
			
		{
			
			return queue.Dequeue();
			
		}
		
	}

	
	public void Clear()
		
	{
		
		lock(syncLock)
			
		{
			
			queue.Clear();
			
		}
		
	}
	
	
	
		
}
