using System.Collections;
using System.Collections.Generic;

namespace Capstone_Project.Input
{
    public class ActionBuffer
    {
        public bool IsEmpty => Count == 0;
        public bool IsFull => Count >= arr.Length;
        public int Count { get; private set; }

        private Action[] arr;
        private int head = 0;
        private int tail = 0;

        public ActionBuffer(int capacity)
        {
            arr = new Action[capacity];
        }

        public Action Peek()
        {
            if (IsEmpty)
                throw new System.IndexOutOfRangeException("ActionBuffer is empty");

            return arr[head];
        }

        public Action PeekTail()
        {
            if (IsEmpty)
                throw new System.IndexOutOfRangeException("ActionBuffer is empty");

            return arr[(tail - 1) % arr.Length];
        }

        // this circular queue Add() implementation is specific to an ActionBuffer, where the last action gets overwritten by the next added Action
        public void Add(Action action)
        {
            if (IsFull)
                arr[tail] = action;
            else
            {
                arr[tail++] = action;
                Count++;
                tail %= arr.Length;
            }
        }

        public Action Remove()
        {
            if (IsEmpty)
                throw new System.IndexOutOfRangeException("ActionBuffer is empty");

            Action removedAction = arr[head];
            arr[head++] = null;
            Count--;
            head %= arr.Length;

            return removedAction;
        }
    }
}
