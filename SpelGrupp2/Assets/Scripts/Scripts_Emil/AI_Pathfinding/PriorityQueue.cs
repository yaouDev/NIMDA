using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reused (but modified) heap from ALDA
public class PriorityQueue<T> {
    private const int DEFAULT_CAPACITY = 50;
    private int numberOfChildren, currentSize;
    private KeyValuePair<T, float>[] array;
    private HashSet<T> contentCheckSet;

    public PriorityQueue() {
        numberOfChildren = 2;
        array = new KeyValuePair<T, float>[DEFAULT_CAPACITY];
        contentCheckSet = new HashSet<T>();
    }

    public int GetParentIndex(int childToCheck) {
        if (childToCheck > 1) {
            int parentIndex;
            if ((childToCheck - 1) % numberOfChildren != 0) {
                parentIndex = childToCheck;
                while (parentIndex % numberOfChildren != 0) parentIndex++;
            } else parentIndex = childToCheck - 1;
            return parentIndex / numberOfChildren;
        }
        throw new System.Exception("Illegal argument: Cannot check parent of root!");
    }

    public bool Contains(T element) {
        return contentCheckSet.Contains(element);
    }

    public int GetFirstChildIndex(int parent) {
        if (parent > 0) return parent * numberOfChildren - (numberOfChildren - 2);
        throw new System.Exception("Illegal argument: Cannot check child of index 0 or below!");
    }

    public int Size() { return currentSize; }

    public void Insert(T element, float priority) {
        if (currentSize == array.Length - 1) EnlargeArray(array.Length * 2 + 1);
        int hole = ++currentSize;
        KeyValuePair<T, float> insertPair = new KeyValuePair<T, float>(element, priority);
        for (array[0] = insertPair; hole > 1 && insertPair.Value < array[GetParentIndex(hole)].Value; hole = GetParentIndex(hole)) {
            array[hole] = array[GetParentIndex(hole)];
        }
        array[hole] = insertPair;
        contentCheckSet.Add(element);
    }

    private void EnlargeArray(int newSize) {
        KeyValuePair<T, float>[] old = array;
        array = new KeyValuePair<T, float>[newSize];
        for (int i = 0; i < old.Length; i++) {
            array[i] = old[i];
        }
    }

    public T FindMin() {
        if (IsEmpty()) throw new System.Exception("Underflow, Queue was empty!");
        return array[1].Key;
    }

    public T DeleteMin() {
        if (IsEmpty()) throw new System.Exception("Underflow, Queue was empty!");
        T minItem = FindMin();
        array[1] = array[currentSize--];
        PercolateDown(1);
        contentCheckSet.Remove(minItem);
        return minItem;
    }

    public bool IsEmpty() { return currentSize == 0; }

    public void MakeEmpty() { currentSize = 0; }

    private void PercolateDown(int hole) {
        int child = 0;
        KeyValuePair<T, float> tmp = array[hole];
        for (; GetFirstChildIndex(hole) <= currentSize; hole = child) {
            child = FindIndexOfMinChild(hole);
            if (child > -1 && array[child].Value < tmp.Value) array[hole] = array[child];
            else break;
        }
        array[hole] = tmp;
    }

    private int FindIndexOfMinChild(int parent) {
        int tmpChild = GetFirstChildIndex(parent);
        if (array.Length > tmpChild && !array[tmpChild].Equals(null)) {
            int child = tmpChild;
            for (int i = 0; i < numberOfChildren && i + tmpChild <= currentSize; i++) {
                if (!array[tmpChild + 1].Equals(null) && array[tmpChild + i].Value < array[child].Value) {
                    child = tmpChild + i;
                }
            }
            return child;
        }
        return -1;
    }
}