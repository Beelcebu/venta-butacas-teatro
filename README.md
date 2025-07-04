# 🎭 Sistema de Venta de Butacas - Teatro

Sistema moderno de venta de entradas para teatro desarrollado en **Blazor WebAssembly** con integración de **MercadoPago** para procesamiento de pagos.

## ✨ Características

- 🎯 **Selección visual de asientos** - Interfaz intuitiva para elegir butacas
- 🎨 **3 categorías de asientos** con precios diferenciados
- 💳 **Integración con MercadoPago** (Checkout Pro)
- 📱 **Diseño responsive** - Funciona en móviles y desktop
- ⚡ **Blazor WebAssembly** - Aplicación web moderna sin necesidad de servidor
- 🎪 **Mapa realista del teatro** con distribución de filas curvas

## 🏗️ Tecnologías

- **Frontend**: Blazor WebAssembly (.NET 8)
- **Estilos**: CSS3 con gradientes y animaciones
- **Pagos**: MercadoPago API
- **Hosting**: Compatible con GitHub Pages, Netlify, Vercel

## 🚀 Instalación y Uso

### Prerrequisitos
- .NET 8 SDK
- Cuenta de MercadoPago (para obtener Access Token)

### Configuración

1. **Clona el repositorio**
   ```bash
   git clone https://github.com/TU_USUARIO/venta-butacas-teatro.git
   cd venta-butacas-teatro
   ```

2. **Configura MercadoPago**
   
   Edita el archivo `appsettings.json` y agrega tu Access Token:
   ```json
   {
     "MercadoPago": {
       "AccessToken": "TU_ACCESS_TOKEN_AQUI"
     }
   }
   ```

3. **Ejecuta la aplicación**
   ```bash
   dotnet run
   ```

4. **Abre tu navegador**
   
   Ve a `http://localhost:5054`

## 📦 Deployment

### GitHub Pages (Recomendado para este proyecto)

1. **Fork/Clona este repositorio**
2. **Habilita GitHub Pages** en la configuración del repositorio
3. **Configura GitHub Actions** (incluido en el proyecto)
4. **Push tus cambios** - El sitio se desplegará automáticamente

### Otras opciones
- **Netlify**: Drag & drop de la carpeta `wwwroot` después de `dotnet publish`
- **Vercel**: Conecta tu repositorio de GitHub
- **Azure Static Web Apps**: Deployment directo desde GitHub

## 🎭 Categorías de Asientos

| Categoría | Color | Descripción | Precio |
|-----------|-------|-------------|---------|
| **VIP Premium** | 🟡 Dorado | Primera fila - La mejor experiencia | $150,000 |
| **Platea** | 🔴 Rojo | Zona central - Excelente vista | $100,000 |
| **General** | 🔵 Azul | Zona posterior - Precio accesible | $50,000 |

## 🛠️ Estructura del Proyecto

```
├── Components/
│   └── TheaterMap.razor      # Componente principal del mapa del teatro
├── Pages/
│   ├── PaymentSuccess.razor  # Página de pago exitoso
│   ├── PaymentFailure.razor  # Página de pago fallido
│   └── PaymentPending.razor  # Página de pago pendiente
├── Services/
│   ├── IMercadoPagoService.cs
│   └── MercadoPagoDirectService.cs
├── wwwroot/
│   └── css/
│       └── theater.css       # Estilos del teatro
└── appsettings.json         # Configuración
```

## 🔧 Configuración de MercadoPago

1. **Crea una cuenta** en [MercadoPago Developers](https://www.mercadopago.com.ar/developers)
2. **Obtén tus credenciales** desde el panel de desarrolladores
3. **Usa el Access Token de prueba** para desarrollo
4. **Cambia al Access Token de producción** para usar en vivo

### URLs de retorno
El sistema está configurado para manejar estos estados de pago:
- ✅ **Éxito**: `/payment/success`
- ❌ **Fallo**: `/payment/failure`
- ⏳ **Pendiente**: `/payment/pending`

## 📱 Screenshots

![Teatro Principal](screenshot-teatro.png)
*Interfaz principal de selección de asientos*

## 🤝 Contribuir

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver `LICENSE` para más detalles.

## 🆘 Soporte

Si tienes alguna pregunta o problema:
- 🐛 **Issues**: [GitHub Issues](https://github.com/TU_USUARIO/venta-butacas-teatro/issues)
- 📧 **Email**: tu-email@ejemplo.com

---

⭐ Si este proyecto te fue útil, ¡dale una estrella en GitHub!
