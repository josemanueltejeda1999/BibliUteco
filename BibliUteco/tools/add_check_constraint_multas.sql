ALTER TABLE Multas
ADD CONSTRAINT CK_Multas_Estado
CHECK (Estado IN ('Pendiente', 'Pagada'));