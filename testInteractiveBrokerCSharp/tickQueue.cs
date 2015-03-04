using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;                           // mysql linkage

namespace testInteractiveBrokerCSharp
{
public class tickQueue
{

    // members
    protected Queue<tick> queue = new Queue<tick>() ;   // Internal queue of db inserts
    protected object qLockObj   = new object()      ;   // Locking Object
    public MySqlConnection conn = null              ;   // Connection object to MySQL
    public bool stop            = false             ;   // Flag to stop processing

    // methods
    public void Add(tick item)                          // Method to enqueue a write
    {
        //Lock for single access only
        lock(qLockObj)
        {
            queue.Enqueue(item);
        }
    }

    public void Run()                                   // Method to write items
    {
        int n = 0;

        while (!stop)                                   // Loop while the stop flag is false
        {
            lock (qLockObj)                             // Lock and get a count on the queue
            {
                n = queue.Count;
            }

            if (n > 0)                                  // If there are objects, process them
            {
                process();
            }

            System.Threading.Thread.Sleep(10);          // Sleep for 10 ms before looping again
        }

        System.Diagnostics.Debug.Write(                 // log any value still in the queue and shutdown
            "Shutting Down tickQueue; " 
            + queue.Count 
            + " items left"
            + System.Environment.NewLine);

        process();
    }

    private void process()                              // Method to process items in the queue
    {
        List<tick> inserts = new List<tick>();
        
        int i = 0;

        lock (qLockObj)                                 // Loop through the queue and put them in a list
        {
            for (i = 0; i < queue.Count; i++)
                inserts.Add(queue.Dequeue());
        }

        System.Diagnostics.Debug.Write(
            "Processing " 
            + i 
            + " items into database"
            + System.Environment.NewLine);

        foreach (tick t in inserts)                     // call insert for each item.
            insert(t);
    }

    private void insert(tick t)                         // Method to insert a tick, TODO: bulk insert method
    {
        using (MySqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = t.insertStr();

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.Write(
                    "an exception occured: "
                    + exp.Message);
            }
        }
    }
}
}
