/*using system;
using system.collections.generic;

public class objectpool<t>
{
    private queue<t> _pool; // cola para almacenar los objetos
    private func<t> _createfunc; // funci�n para crear nuevos objetos
    private action<t> _onget; // acci�n al obtener un objeto del pool
    private action<t> _onrelease; // acci�n al devolver un objeto al pool

    /// <summary>
    /// constructor del object pool.
    /// </summary>
    /// <param name="createfunc">funci�n para crear nuevos objetos.</param>
    /// <param name="onget">acci�n al obtener un objeto del pool.</param>
    /// <param name="onrelease">acci�n al devolver un objeto al pool.</param>
    /// <param name="initialsize">tama�o inicial del pool.</param>
    public objectpool(func<t> createfunc, action<t> onget = null, action<t> onrelease = null, int initialsize = 10)
    {
        _pool = new queue<t>(initialsize);
        _createfunc = createfunc ?? throw new argumentnullexception(nameof(createfunc));
        _onget = onget;
        _onrelease = onrelease;

        // pre-cargar el pool con objetos iniciales
        for (int i = 0; i < initialsize; i++)
        {
            _pool.enqueue(_createfunc());
        }
    }

    /// <summary>
    /// obtiene un objeto del pool.
    /// </summary>
    public t get()
    {
        t obj;
        if (_pool.count > 0)
        {
            obj = _pool.dequeue();
        }
        else
        {
            obj = _createfunc(); // crear un nuevo objeto si el pool est� vac�o
        }

        _onget?.invoke(obj); // ejecutar la acci�n onget
        return obj;
    }

    /// <summary>
    /// devuelve un objeto al pool.
    /// </summary>
    public void release(t obj)
    {
        _onrelease?.invoke(obj); // ejecutar la acci�n onrelease
        _pool.enqueue(obj);
    }

}*/

using System.Collections.Generic;
using System;

public class ObjectPool<T> where T : class
{
    private Queue<T> _pool; // Cola para almacenar los objetos inactivos
    private HashSet<T> _activeObjects; // Conjunto para almacenar los objetos activos
    private Func<T> _createFunc; // Funci�n para crear nuevos objetos
    private Action<T> _onGet; // Acci�n al obtener un objeto del pool
    private Action<T> _onRelease; // Acci�n al devolver un objeto al pool

    public ObjectPool(Func<T> createFunc, Action<T> onGet = null, Action<T> onRelease = null, int initialSize = 10)
    {
        _pool = new Queue<T>(initialSize);
        _activeObjects = new HashSet<T>();
        _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
        _onGet = onGet;
        _onRelease = onRelease;

        // Pre-cargar el pool con objetos iniciales
        for (int i = 0; i < initialSize; i++)
        {
            _pool.Enqueue(_createFunc());
        }
    }

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

        // Registrar el objeto como activo
        _activeObjects.Add(obj);

        // Ejecutar la acci�n OnGet
        _onGet?.Invoke(obj);

        return obj;
    }

    public void Release(T obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        // Verificar si el objeto est� activo
        if (!_activeObjects.Contains(obj))
        {
            throw new InvalidOperationException("El objeto no est� activo o ya fue liberado.");
        }

        // Ejecutar la acci�n OnRelease
        _onRelease?.Invoke(obj);

        // Marcar el objeto como inactivo
        _activeObjects.Remove(obj);

        // Devolver el objeto al pool
        _pool.Enqueue(obj);
    }

    /// <summary>
    /// Devuelve todos los objetos activos al pool, reiniciando su estado y desactiv�ndolos si es necesario.
    /// </summary>
    public void ReleaseAll()
    {
        // Recorre todos los objetos que est�n fuera del pool
        foreach (var obj in _activeObjects)
        {
            _onRelease?.Invoke(obj); // Ejecutar la acci�n OnRelease (si est� definida)
            _pool.Enqueue(obj); // Devolver el objeto al pool
        }

        _activeObjects.Clear(); // Limpiar la lista de objetos activos
    }
}