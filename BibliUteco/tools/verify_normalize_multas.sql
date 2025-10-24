SELECT TOP 50 MultaId, PrestamoId, Estado, Monto, FechaGenerada
FROM Multas
ORDER BY FechaGenerada DESC;

SELECT Estado, COUNT(*) AS Cnt
FROM Multas
GROUP BY Estado
ORDER BY Cnt DESC;