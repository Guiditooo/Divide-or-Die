using System;
using System.Collections.Generic;

public class ObjectPool<T>
{
    private Queue<T> _pool; // Cola para almacenar los objetos
    private Func<T> _createFunc; // Funci�n para crear nuevos objetos
    private Action<T> _onGet; // Acci�n al obtener un objeto del pool
    private Action<T> _onRelease; // Acci�n al devolver un objeto al pool

    /// <summary>
    /// Constructor del Object Pool.
    /// </summary>
    /// <param name="createFunc">Funci�n para crear nuevos objetos.</param>
    /// <param name="onGet">Acci�n al obtener un objeto del pool.</param>
    /// <param name="onRelease">Acci�n al devolver un objeto al pool.</param>
    /// <param name="initialSize">Tama�o inicial del pool.</param>
    public ObjectPool(Func<T> createFunc, Action<T> onGet = null, Action<T> onRelease = null, int initialSize = 10)
    {
        _pool = new Queue<T>(initialSize);
        _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
        _onGet = onGet;
        _onRelease = onRelease;

        // Pre-cargar el pool con objetos iniciales
        for (int i = 0; i < initialSize; i++)
        {
            _pool.Enqueue(_createFunc());
        }
    }

    /// <summary>
    /// Obtiene un objeto del pool.
    /// </summary>
    public T Get()
    {
        T obj;
        if (_pool.Count > 0)
        {
            obj = _pool.Dequeue();
        }
        else
        {
            obj = _createFunc(); // Crear un nuevo objeto si el pool est� vac�o
        }

        _onGet?.Invoke(obj); // Ejecutar la acci�n OnGet
        return obj;
    }

    /// <summary>
    /// Devuelve un objeto al pool.
    /// </summary>
    public void Release(T obj)
    {
        _onRelease?.Invoke(obj); // Ejecutar la acci�n OnRelease
        _pool.Enqueue(obj);
    }

}