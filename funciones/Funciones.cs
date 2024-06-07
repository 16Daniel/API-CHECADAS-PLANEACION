using API_PEDIDOS.Controllers;

namespace API_PEDIDOS.funciones
{
    public static class Funciones
    {
        public static Boolean LineasNegativas(List<ArticuloPedido> articulos)
        {
            Boolean lineasnegativas = false;
            foreach (var item in articulos)
            {
                if (item.total_linea < 0)
                {
                    lineasnegativas = true;
                }
            }
            return lineasnegativas;
        }
    }
}
