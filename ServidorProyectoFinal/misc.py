
def split(l, size):
    """
    Divide una lista en porciones de un tamano dado
    :param l: Lista a dividir
    :param size: Tamano deseado
    :return: Lista de listas del tamano deseado (o menor)
    """
    arrs = []
    while len(l) > size:
        pice = l[:size]
        arrs.append(pice)
        l = l[size:]
    arrs.append(l)
    return arrs
