USE [master]
GO
/****** Object:  Database [ComplejoDeportivo]    Script Date: 4/11/2025 13:33:06 ******/
CREATE DATABASE [ComplejoDeportivo]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ComplejoDeportivo', FILENAME = N'C:\Users\marti\ComplejoDeportivo.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'ComplejoDeportivo_log', FILENAME = N'C:\Users\marti\ComplejoDeportivo_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [ComplejoDeportivo] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ComplejoDeportivo].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ComplejoDeportivo] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ComplejoDeportivo] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ComplejoDeportivo] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ComplejoDeportivo] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ComplejoDeportivo] SET ARITHABORT OFF 
GO
ALTER DATABASE [ComplejoDeportivo] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [ComplejoDeportivo] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ComplejoDeportivo] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ComplejoDeportivo] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ComplejoDeportivo] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ComplejoDeportivo] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ComplejoDeportivo] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ComplejoDeportivo] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ComplejoDeportivo] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ComplejoDeportivo] SET  ENABLE_BROKER 
GO
ALTER DATABASE [ComplejoDeportivo] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ComplejoDeportivo] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ComplejoDeportivo] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ComplejoDeportivo] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ComplejoDeportivo] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ComplejoDeportivo] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ComplejoDeportivo] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ComplejoDeportivo] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [ComplejoDeportivo] SET  MULTI_USER 
GO
ALTER DATABASE [ComplejoDeportivo] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ComplejoDeportivo] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ComplejoDeportivo] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ComplejoDeportivo] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [ComplejoDeportivo] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [ComplejoDeportivo] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [ComplejoDeportivo] SET QUERY_STORE = OFF
GO
USE [ComplejoDeportivo]
GO
/****** Object:  Table [dbo].[Almacen]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Almacen](
	[AlmacenID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](100) NOT NULL,
	[ComplejoID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AlmacenID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Asador]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Asador](
	[AsadorID] [int] IDENTITY(1,1) NOT NULL,
	[ComplejoID] [int] NOT NULL,
	[Nombre] [nvarchar](100) NOT NULL,
	[PrecioHora] [decimal](10, 2) NOT NULL,
	[Activo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AsadorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BloqueoCancha]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BloqueoCancha](
	[BloqueoID] [int] IDENTITY(1,1) NOT NULL,
	[CanchaID] [int] NOT NULL,
	[Fecha] [date] NOT NULL,
	[HoraInicio] [time](7) NOT NULL,
	[HoraFin] [time](7) NOT NULL,
	[Motivo] [nvarchar](200) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BloqueoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CalendarioFeriados]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CalendarioFeriados](
	[FeriadoID] [int] IDENTITY(1,1) NOT NULL,
	[Fecha] [date] NOT NULL,
	[Descripcion] [nvarchar](150) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[FeriadoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cancha]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cancha](
	[CanchaID] [int] IDENTITY(1,1) NOT NULL,
	[ComplejoID] [int] NOT NULL,
	[TipoCanchaID] [int] NOT NULL,
	[TipoSuperficieID] [int] NOT NULL,
	[Nombre] [nvarchar](50) NOT NULL,
	[Activa] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CanchaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CategoriaProducto]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CategoriaProducto](
	[CategoriaID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CategoriaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cliente]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cliente](
	[ClienteID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](100) NOT NULL,
	[Apellido] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](150) NULL,
	[Telefono] [nvarchar](20) NULL,
	[Documento] [nvarchar](20) NULL,
	[FechaRegistro] [date] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ClienteID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Complejo]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Complejo](
	[ComplejoID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](100) NOT NULL,
	[DireccionID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ComplejoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DetalleReserva]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetalleReserva](
	[DetalleReservaID] [int] IDENTITY(1,1) NOT NULL,
	[ReservaID] [int] NOT NULL,
	[CanchaID] [int] NOT NULL,
	[TarifaHoraID] [int] NOT NULL,
	[CantidadHoras] [int] NOT NULL,
	[Descuento] [decimal](10, 2) NOT NULL,
	[Recargo] [decimal](10, 2) NOT NULL,
	[Subtotal] [decimal](12, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[DetalleReservaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DetalleReservaAsador]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DetalleReservaAsador](
	[DetalleRAID] [int] IDENTITY(1,1) NOT NULL,
	[ReservaAsadorID] [int] NOT NULL,
	[AsadorID] [int] NOT NULL,
	[PrecioHora] [decimal](10, 2) NOT NULL,
	[CantidadHoras] [int] NOT NULL,
	[Subtotal] [decimal](10, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[DetalleRAID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Direccion]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Direccion](
	[DireccionID] [int] IDENTITY(1,1) NOT NULL,
	[Calle] [nvarchar](150) NOT NULL,
	[Numero] [nvarchar](10) NOT NULL,
	[Ciudad] [nvarchar](50) NOT NULL,
	[Provincia] [nvarchar](50) NOT NULL,
	[CodigoPostal] [nvarchar](10) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[DireccionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Empleado]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Empleado](
	[EmpleadoID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](100) NOT NULL,
	[Apellido] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](150) NULL,
	[Telefono] [nvarchar](20) NULL,
	[Cargo] [nvarchar](100) NOT NULL,
	[FechaIngreso] [date] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[EmpleadoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EstadoPago]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EstadoPago](
	[EstadoPagoID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[EstadoPagoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EstadoReserva]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EstadoReserva](
	[EstadoReservaID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[EstadoReservaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Factura]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Factura](
	[FacturaID] [int] IDENTITY(1,1) NOT NULL,
	[ClienteID] [int] NOT NULL,
	[TipoComprobanteID] [int] NOT NULL,
	[Fecha] [date] NOT NULL,
	[NumeroComprobante] [nvarchar](20) NOT NULL,
	[Total] [decimal](12, 2) NOT NULL,
	[FechaCreacion] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[FacturaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FacturaDetalle]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FacturaDetalle](
	[FacturaDetalleID] [int] IDENTITY(1,1) NOT NULL,
	[FacturaID] [int] NOT NULL,
	[ReservaID] [int] NULL,
	[PedidoID] [int] NULL,
	[ReservaAsadorID] [int] NULL,
	[Descripcion] [nvarchar](200) NOT NULL,
	[Subtotal] [decimal](12, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[FacturaDetalleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FormaPago]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FormaPago](
	[FormaPagoID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[FormaPagoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MovimientoStock]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MovimientoStock](
	[MovimientoID] [int] IDENTITY(1,1) NOT NULL,
	[ProductoID] [int] NOT NULL,
	[AlmacenID] [int] NOT NULL,
	[Cantidad] [int] NOT NULL,
	[Fecha] [datetime] NOT NULL,
	[TipoMovimientoID] [int] NOT NULL,
	[Observaciones] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[MovimientoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Pago]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Pago](
	[PagoID] [int] IDENTITY(1,1) NOT NULL,
	[FacturaID] [int] NOT NULL,
	[FormaPagoID] [int] NOT NULL,
	[EstadoPagoID] [int] NOT NULL,
	[Monto] [decimal](12, 2) NOT NULL,
	[FechaPago] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PagoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PedidoKiosko]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PedidoKiosko](
	[PedidoID] [int] IDENTITY(1,1) NOT NULL,
	[ClienteID] [int] NOT NULL,
	[Fecha] [datetime] NOT NULL,
	[Total] [decimal](12, 2) NOT NULL,
	[Estado] [nvarchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PedidoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PedidoKioskoDetalle]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PedidoKioskoDetalle](
	[DetalleID] [int] IDENTITY(1,1) NOT NULL,
	[PedidoID] [int] NOT NULL,
	[ProductoID] [int] NOT NULL,
	[Cantidad] [int] NOT NULL,
	[PrecioUnitario] [decimal](10, 2) NOT NULL,
	[Subtotal] [decimal](12, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[DetalleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Producto]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Producto](
	[ProductoID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](100) NOT NULL,
	[CategoriaID] [int] NOT NULL,
	[UnidadID] [int] NOT NULL,
	[PrecioBase] [decimal](10, 2) NOT NULL,
	[Activo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductoProveedor]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductoProveedor](
	[ProdProvID] [int] IDENTITY(1,1) NOT NULL,
	[ProductoID] [int] NOT NULL,
	[ProveedorID] [int] NOT NULL,
	[PrecioCompra] [decimal](10, 2) NOT NULL,
	[EsPrincipal] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ProdProvID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Proveedor]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Proveedor](
	[ProveedorID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](100) NOT NULL,
	[Telefono] [nvarchar](50) NULL,
	[Email] [nvarchar](100) NULL,
	[CUIT] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[ProveedorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProveedorContacto]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProveedorContacto](
	[ContactoID] [int] IDENTITY(1,1) NOT NULL,
	[ProveedorID] [int] NOT NULL,
	[Nombre] [nvarchar](100) NOT NULL,
	[Telefono] [nvarchar](50) NULL,
	[Email] [nvarchar](100) NULL,
	[Cargo] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ContactoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Reserva]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reserva](
	[ReservaID] [int] IDENTITY(1,1) NOT NULL,
	[ClienteID] [int] NOT NULL,
	[Fecha] [date] NOT NULL,
	[HoraInicio] [time](7) NOT NULL,
	[HoraFin] [time](7) NOT NULL,
	[EstadoReservaID] [int] NOT NULL,
	[Ambito] [nvarchar](20) NOT NULL,
	[FechaCreacion] [datetime] NOT NULL,
	[Total] [decimal](12, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ReservaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ReservaAsador]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReservaAsador](
	[ReservaAsadorID] [int] IDENTITY(1,1) NOT NULL,
	[ClienteID] [int] NOT NULL,
	[Fecha] [date] NOT NULL,
	[HoraInicio] [time](7) NOT NULL,
	[HoraFin] [time](7) NOT NULL,
	[EstadoReservaID] [int] NOT NULL,
	[Total] [decimal](10, 2) NOT NULL,
	[FechaCreacion] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ReservaAsadorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Stock]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Stock](
	[StockID] [int] IDENTITY(1,1) NOT NULL,
	[ProductoID] [int] NOT NULL,
	[AlmacenID] [int] NOT NULL,
	[Cantidad] [int] NOT NULL,
	[StockMinimo] [int] NOT NULL,
	[StockMaximo] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[StockID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tarifa]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tarifa](
	[TarifaID] [int] IDENTITY(1,1) NOT NULL,
	[CanchaID] [int] NOT NULL,
	[Precio] [decimal](10, 2) NOT NULL,
	[ContratoLuz] [bit] NOT NULL,
	[FechaVigencia] [date] NOT NULL,
	[EsActual] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TarifaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TipoCancha]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipoCancha](
	[TipoCanchaID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TipoCanchaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TipoComprobante]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipoComprobante](
	[TipoComprobanteID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](50) NOT NULL,
	[CodigoAFIP] [nvarchar](10) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TipoComprobanteID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TipoMovimientoStock]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipoMovimientoStock](
	[TipoMovimientoID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](50) NOT NULL,
	[AfectaStock] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TipoMovimientoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TipoSuperficie]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipoSuperficie](
	[TipoSuperficieID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TipoSuperficieID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UnidadMedida]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UnidadMedida](
	[UnidadID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [nvarchar](50) NOT NULL,
	[Abreviatura] [nvarchar](10) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UnidadID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Usuario]    Script Date: 4/11/2025 13:33:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usuario](
	[UsuarioID] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](150) NOT NULL,
	[PasswordHash] [nvarchar](255) NOT NULL,
	[TipoUsuario] [nvarchar](20) NOT NULL,
	[ClienteID] [int] NULL,
	[EmpleadoID] [int] NULL,
	[FechaRegistro] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UsuarioID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [idx_factura_fecha]    Script Date: 4/11/2025 13:33:06 ******/
CREATE NONCLUSTERED INDEX [idx_factura_fecha] ON [dbo].[Factura]
(
	[Fecha] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [idx_movimientostock_fecha]    Script Date: 4/11/2025 13:33:06 ******/
CREATE NONCLUSTERED INDEX [idx_movimientostock_fecha] ON [dbo].[MovimientoStock]
(
	[Fecha] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [idx_pedido_fecha]    Script Date: 4/11/2025 13:33:06 ******/
CREATE NONCLUSTERED INDEX [idx_pedido_fecha] ON [dbo].[PedidoKiosko]
(
	[Fecha] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [idx_precioproducto_vigencia]    Script Date: 4/11/2025 13:33:06 ******/
CREATE NONCLUSTERED INDEX [idx_precioproducto_vigencia] ON [dbo].[Producto]
(
	[ProductoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [idx_reserva_cliente]    Script Date: 4/11/2025 13:33:06 ******/
CREATE NONCLUSTERED INDEX [idx_reserva_cliente] ON [dbo].[Reserva]
(
	[ClienteID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [idx_reserva_fecha]    Script Date: 4/11/2025 13:33:06 ******/
CREATE NONCLUSTERED INDEX [idx_reserva_fecha] ON [dbo].[Reserva]
(
	[Fecha] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [idx_reservaasador_fecha]    Script Date: 4/11/2025 13:33:06 ******/
CREATE NONCLUSTERED INDEX [idx_reservaasador_fecha] ON [dbo].[ReservaAsador]
(
	[Fecha] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [idx_stock_producto]    Script Date: 4/11/2025 13:33:06 ******/
CREATE NONCLUSTERED INDEX [idx_stock_producto] ON [dbo].[Stock]
(
	[ProductoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [idx_tarifa_vigencia]    Script Date: 4/11/2025 13:33:06 ******/
CREATE NONCLUSTERED INDEX [idx_tarifa_vigencia] ON [dbo].[Tarifa]
(
	[FechaVigencia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [idx_usuario_cliente]    Script Date: 4/11/2025 13:33:06 ******/
CREATE NONCLUSTERED INDEX [idx_usuario_cliente] ON [dbo].[Usuario]
(
	[ClienteID] ASC
)
WHERE ([ClienteID] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [idx_usuario_email]    Script Date: 4/11/2025 13:33:06 ******/
CREATE UNIQUE NONCLUSTERED INDEX [idx_usuario_email] ON [dbo].[Usuario]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [idx_usuario_empleado]    Script Date: 4/11/2025 13:33:06 ******/
CREATE NONCLUSTERED INDEX [idx_usuario_empleado] ON [dbo].[Usuario]
(
	[EmpleadoID] ASC
)
WHERE ([EmpleadoID] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Asador] ADD  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[Cancha] ADD  DEFAULT ((1)) FOR [Activa]
GO
ALTER TABLE [dbo].[DetalleReserva] ADD  DEFAULT ((0)) FOR [Descuento]
GO
ALTER TABLE [dbo].[DetalleReserva] ADD  DEFAULT ((0)) FOR [Recargo]
GO
ALTER TABLE [dbo].[Producto] ADD  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[ProductoProveedor] ADD  DEFAULT ((0)) FOR [EsPrincipal]
GO
ALTER TABLE [dbo].[Stock] ADD  DEFAULT ((0)) FOR [StockMinimo]
GO
ALTER TABLE [dbo].[Stock] ADD  DEFAULT ((0)) FOR [StockMaximo]
GO
ALTER TABLE [dbo].[Tarifa] ADD  DEFAULT ((1)) FOR [EsActual]
GO
ALTER TABLE [dbo].[Usuario] ADD  DEFAULT (getdate()) FOR [FechaRegistro]
GO
ALTER TABLE [dbo].[Almacen]  WITH CHECK ADD FOREIGN KEY([ComplejoID])
REFERENCES [dbo].[Complejo] ([ComplejoID])
GO
ALTER TABLE [dbo].[Asador]  WITH CHECK ADD FOREIGN KEY([ComplejoID])
REFERENCES [dbo].[Complejo] ([ComplejoID])
GO
ALTER TABLE [dbo].[BloqueoCancha]  WITH CHECK ADD FOREIGN KEY([CanchaID])
REFERENCES [dbo].[Cancha] ([CanchaID])
GO
ALTER TABLE [dbo].[Cancha]  WITH CHECK ADD FOREIGN KEY([ComplejoID])
REFERENCES [dbo].[Complejo] ([ComplejoID])
GO
ALTER TABLE [dbo].[Cancha]  WITH CHECK ADD FOREIGN KEY([TipoCanchaID])
REFERENCES [dbo].[TipoCancha] ([TipoCanchaID])
GO
ALTER TABLE [dbo].[Cancha]  WITH CHECK ADD FOREIGN KEY([TipoSuperficieID])
REFERENCES [dbo].[TipoSuperficie] ([TipoSuperficieID])
GO
ALTER TABLE [dbo].[Complejo]  WITH CHECK ADD FOREIGN KEY([DireccionID])
REFERENCES [dbo].[Direccion] ([DireccionID])
GO
ALTER TABLE [dbo].[DetalleReserva]  WITH CHECK ADD FOREIGN KEY([CanchaID])
REFERENCES [dbo].[Cancha] ([CanchaID])
GO
ALTER TABLE [dbo].[DetalleReserva]  WITH CHECK ADD FOREIGN KEY([ReservaID])
REFERENCES [dbo].[Reserva] ([ReservaID])
GO
ALTER TABLE [dbo].[DetalleReserva]  WITH CHECK ADD FOREIGN KEY([TarifaHoraID])
REFERENCES [dbo].[Tarifa] ([TarifaID])
GO
ALTER TABLE [dbo].[DetalleReservaAsador]  WITH CHECK ADD FOREIGN KEY([AsadorID])
REFERENCES [dbo].[Asador] ([AsadorID])
GO
ALTER TABLE [dbo].[DetalleReservaAsador]  WITH CHECK ADD FOREIGN KEY([ReservaAsadorID])
REFERENCES [dbo].[ReservaAsador] ([ReservaAsadorID])
GO
ALTER TABLE [dbo].[Factura]  WITH CHECK ADD FOREIGN KEY([ClienteID])
REFERENCES [dbo].[Cliente] ([ClienteID])
GO
ALTER TABLE [dbo].[Factura]  WITH CHECK ADD FOREIGN KEY([TipoComprobanteID])
REFERENCES [dbo].[TipoComprobante] ([TipoComprobanteID])
GO
ALTER TABLE [dbo].[FacturaDetalle]  WITH CHECK ADD FOREIGN KEY([FacturaID])
REFERENCES [dbo].[Factura] ([FacturaID])
GO
ALTER TABLE [dbo].[FacturaDetalle]  WITH CHECK ADD FOREIGN KEY([PedidoID])
REFERENCES [dbo].[PedidoKiosko] ([PedidoID])
GO
ALTER TABLE [dbo].[FacturaDetalle]  WITH CHECK ADD FOREIGN KEY([ReservaID])
REFERENCES [dbo].[Reserva] ([ReservaID])
GO
ALTER TABLE [dbo].[FacturaDetalle]  WITH CHECK ADD FOREIGN KEY([ReservaAsadorID])
REFERENCES [dbo].[ReservaAsador] ([ReservaAsadorID])
GO
ALTER TABLE [dbo].[MovimientoStock]  WITH CHECK ADD FOREIGN KEY([AlmacenID])
REFERENCES [dbo].[Almacen] ([AlmacenID])
GO
ALTER TABLE [dbo].[MovimientoStock]  WITH CHECK ADD FOREIGN KEY([ProductoID])
REFERENCES [dbo].[Producto] ([ProductoID])
GO
ALTER TABLE [dbo].[MovimientoStock]  WITH CHECK ADD FOREIGN KEY([TipoMovimientoID])
REFERENCES [dbo].[TipoMovimientoStock] ([TipoMovimientoID])
GO
ALTER TABLE [dbo].[Pago]  WITH CHECK ADD FOREIGN KEY([EstadoPagoID])
REFERENCES [dbo].[EstadoPago] ([EstadoPagoID])
GO
ALTER TABLE [dbo].[Pago]  WITH CHECK ADD FOREIGN KEY([FacturaID])
REFERENCES [dbo].[Factura] ([FacturaID])
GO
ALTER TABLE [dbo].[Pago]  WITH CHECK ADD FOREIGN KEY([FormaPagoID])
REFERENCES [dbo].[FormaPago] ([FormaPagoID])
GO
ALTER TABLE [dbo].[PedidoKiosko]  WITH CHECK ADD FOREIGN KEY([ClienteID])
REFERENCES [dbo].[Cliente] ([ClienteID])
GO
ALTER TABLE [dbo].[PedidoKioskoDetalle]  WITH CHECK ADD FOREIGN KEY([PedidoID])
REFERENCES [dbo].[PedidoKiosko] ([PedidoID])
GO
ALTER TABLE [dbo].[PedidoKioskoDetalle]  WITH CHECK ADD FOREIGN KEY([ProductoID])
REFERENCES [dbo].[Producto] ([ProductoID])
GO
ALTER TABLE [dbo].[Producto]  WITH CHECK ADD FOREIGN KEY([CategoriaID])
REFERENCES [dbo].[CategoriaProducto] ([CategoriaID])
GO
ALTER TABLE [dbo].[Producto]  WITH CHECK ADD FOREIGN KEY([UnidadID])
REFERENCES [dbo].[UnidadMedida] ([UnidadID])
GO
ALTER TABLE [dbo].[ProductoProveedor]  WITH CHECK ADD FOREIGN KEY([ProductoID])
REFERENCES [dbo].[Producto] ([ProductoID])
GO
ALTER TABLE [dbo].[ProductoProveedor]  WITH CHECK ADD FOREIGN KEY([ProveedorID])
REFERENCES [dbo].[Proveedor] ([ProveedorID])
GO
ALTER TABLE [dbo].[ProveedorContacto]  WITH CHECK ADD FOREIGN KEY([ProveedorID])
REFERENCES [dbo].[Proveedor] ([ProveedorID])
GO
ALTER TABLE [dbo].[Reserva]  WITH CHECK ADD FOREIGN KEY([ClienteID])
REFERENCES [dbo].[Cliente] ([ClienteID])
GO
ALTER TABLE [dbo].[Reserva]  WITH CHECK ADD FOREIGN KEY([EstadoReservaID])
REFERENCES [dbo].[EstadoReserva] ([EstadoReservaID])
GO
ALTER TABLE [dbo].[ReservaAsador]  WITH CHECK ADD FOREIGN KEY([ClienteID])
REFERENCES [dbo].[Cliente] ([ClienteID])
GO
ALTER TABLE [dbo].[ReservaAsador]  WITH CHECK ADD FOREIGN KEY([EstadoReservaID])
REFERENCES [dbo].[EstadoReserva] ([EstadoReservaID])
GO
ALTER TABLE [dbo].[Stock]  WITH CHECK ADD FOREIGN KEY([AlmacenID])
REFERENCES [dbo].[Almacen] ([AlmacenID])
GO
ALTER TABLE [dbo].[Stock]  WITH CHECK ADD FOREIGN KEY([ProductoID])
REFERENCES [dbo].[Producto] ([ProductoID])
GO
ALTER TABLE [dbo].[Tarifa]  WITH CHECK ADD FOREIGN KEY([CanchaID])
REFERENCES [dbo].[Cancha] ([CanchaID])
GO
ALTER TABLE [dbo].[Usuario]  WITH CHECK ADD FOREIGN KEY([ClienteID])
REFERENCES [dbo].[Cliente] ([ClienteID])
GO
ALTER TABLE [dbo].[Usuario]  WITH CHECK ADD FOREIGN KEY([EmpleadoID])
REFERENCES [dbo].[Empleado] ([EmpleadoID])
GO
ALTER TABLE [dbo].[Usuario]  WITH CHECK ADD  CONSTRAINT [CHK_Usuario_Referencia] CHECK  (([TipoUsuario]='Cliente' AND [ClienteID] IS NOT NULL AND [EmpleadoID] IS NULL OR [TipoUsuario]='Empleado' AND [EmpleadoID] IS NOT NULL AND [ClienteID] IS NULL OR [TipoUsuario]='Administrador' AND [ClienteID] IS NULL AND [EmpleadoID] IS NULL))
GO
ALTER TABLE [dbo].[Usuario] CHECK CONSTRAINT [CHK_Usuario_Referencia]
GO
ALTER TABLE [dbo].[Usuario]  WITH CHECK ADD CHECK  (([TipoUsuario]='Administrador' OR [TipoUsuario]='Empleado' OR [TipoUsuario]='Cliente'))
GO
USE [master]
GO
ALTER DATABASE [ComplejoDeportivo] SET  READ_WRITE 
GO
