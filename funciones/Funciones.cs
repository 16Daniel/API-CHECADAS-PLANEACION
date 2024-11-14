using API_PEDIDOS.Controllers;

namespace API_PEDIDOS.funciones
{
    public static class Funciones
    {
        public static Boolean LineasRojas(List<ArticuloPedido> articulos,Boolean tieneretornables, double cartones)
        {
            Boolean lineasrojas = false;

            double cartonesplaneacion = 0;
            double diferenciacartones = 0;

            if (tieneretornables)
            {
                foreach (var itemp in articulos)
                {
                    if (itemp.esretornable)
                    {
                        cartonesplaneacion = cartonesplaneacion + itemp.cajas;
                    }
                }
                diferenciacartones = cartones - cartonesplaneacion;
            }

            if (diferenciacartones < 0)
            {
                lineasrojas = true;
            }

            foreach (var item in articulos)
            {

                if (item.total_linea <= 0)
                {
                    lineasrojas = true;
                }
            }


            return lineasrojas;
        }
    }
}
