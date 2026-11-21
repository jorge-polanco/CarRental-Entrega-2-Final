from docx import Document
from docx.shared import Inches, Pt, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.table import WD_TABLE_ALIGNMENT, WD_CELL_VERTICAL_ALIGNMENT
from docx.oxml import OxmlElement
from docx.oxml.ns import qn
from docx.enum.section import WD_SECTION
from pathlib import Path

ROOT = Path(r'C:\Users\jorge\OneDrive\Documentos\calidad de software\CarRental-master\CarRental-master')
OUT1 = ROOT / 'evidencias' / 'JWT-Seguridad'
OUT2 = ROOT / 'evidencias' / 'ISO-Countries-Customers'

NAVY='17365D'; BLUE='D9EAF7'; PALE='F3F6F9'; GRAY='D9D9D9'; BLACK='000000'

def shade(cell, fill):
    tcPr = cell._tc.get_or_add_tcPr(); shd = OxmlElement('w:shd'); shd.set(qn('w:fill'), fill); tcPr.append(shd)

def borders(table):
    tblPr=table._tbl.tblPr; b=OxmlElement('w:tblBorders')
    for edge in ('top','left','bottom','right','insideH','insideV'):
        el=OxmlElement('w:'+edge); el.set(qn('w:val'),'single'); el.set(qn('w:sz'),'4'); el.set(qn('w:color'),GRAY); b.append(el)
    tblPr.append(b)

def set_cell(cell, text, bold=False, color=BLACK, size=9.5):
    cell.text=''; p=cell.paragraphs[0]; p.paragraph_format.space_after=Pt(1)
    r=p.add_run(text); r.bold=bold; r.font.name='Aptos'; r.font.size=Pt(size); r.font.color.rgb=RGBColor.from_string(color)
    cell.vertical_alignment=WD_CELL_VERTICAL_ALIGNMENT.CENTER

def base(title, subtitle):
    d=Document(); sec=d.sections[0]; sec.top_margin=Inches(.7); sec.bottom_margin=Inches(.65); sec.left_margin=Inches(.75); sec.right_margin=Inches(.75)
    sec.page_width=Inches(8.5); sec.page_height=Inches(11)
    normal=d.styles['Normal']; normal.font.name='Aptos'; normal.font.size=Pt(10.5); normal.font.color.rgb=RGBColor(0,0,0); normal.paragraph_format.space_after=Pt(6)
    for sty,size in [('Title',24),('Heading 1',16),('Heading 2',12)]:
        s=d.styles[sty]; s.font.name='Aptos Display'; s.font.size=Pt(size); s.font.bold=True; s.font.color.rgb=RGBColor(0,0,0)
        s.paragraph_format.space_before=Pt(10); s.paragraph_format.space_after=Pt(5)
        pPr=s._element.get_or_add_pPr(); pBdr=pPr.find(qn('w:pBdr'))
        if pBdr is not None: pPr.remove(pBdr)
    p=d.add_paragraph(style='Title'); p.add_run(title)
    p=d.add_paragraph(); p.paragraph_format.space_after=Pt(12); r=p.add_run(subtitle); r.font.size=Pt(11); r.font.color.rgb=RGBColor(75,85,99)
    hp=sec.header.paragraphs[0]; hp.alignment=WD_ALIGN_PARAGRAPH.RIGHT; rr=hp.add_run('CAR RENTAL  |  EVIDENCIA DE PRÁCTICA'); rr.font.name='Aptos'; rr.font.size=Pt(8); rr.font.color.rgb=RGBColor(90,90,90)
    fp=sec.footer.paragraphs[0]; fp.alignment=WD_ALIGN_PARAGRAPH.CENTER; f=fp.add_run('CarRental · Evidencia de implementación y verificación'); f.font.name='Aptos'; f.font.size=Pt(8); f.font.color.rgb=RGBColor(100,100,100)
    return d

def add_bullets(d, items):
    for item in items:
        p=d.add_paragraph(style='List Bullet'); p.paragraph_format.space_after=Pt(3); p.add_run(item)

def add_table(d, headers, rows, widths=None):
    t=d.add_table(rows=1, cols=len(headers)); t.alignment=WD_TABLE_ALIGNMENT.CENTER; t.autofit=False
    for i,h in enumerate(headers): set_cell(t.rows[0].cells[i],h,True,'FFFFFF',9); shade(t.rows[0].cells[i],NAVY)
    for row in rows:
        cells=t.add_row().cells
        for i,val in enumerate(row):
            set_cell(cells[i],str(val),False,BLACK,9)
            if len(t.rows)%2==0: shade(cells[i],PALE)
    if widths:
        for row in t.rows:
            for i,w in enumerate(widths): row.cells[i].width=Inches(w)
    borders(t); d.add_paragraph().paragraph_format.space_after=Pt(0)
    return t

def caption(d, text):
    p=d.add_paragraph(); p.alignment=WD_ALIGN_PARAGRAPH.CENTER; p.paragraph_format.space_before=Pt(4); p.paragraph_format.space_after=Pt(8)
    r=p.add_run(text); r.italic=True; r.font.size=Pt(9); r.font.color.rgb=RGBColor(80,80,80)

# Document 1 — JWT
j=base('Seguridad JWT en CarRental','Informe de implementación y evidencia de autenticación')
j.add_paragraph('Resumen de la práctica JWT, persistencia de usuarios en MySQL y evidencia de Postman. La captura confirma el login y la emisión del token; no muestra el token ni credenciales. Los demás escenarios de autorización descritos en la guía no se afirman como verificados aquí.')
j.add_heading('Implementación en la solución', level=1)
add_bullets(j,[
    'La API expone registro e inicio de sesión en /api/auth/register y /api/auth/login. Las contraseñas se almacenan con el hasher de ASP.NET Core y el rol inicial se asigna en el servidor.',
    'La configuración de autenticación valida firma, emisor, audiencia y vigencia del JWT. Swagger documenta la autorización Bearer.',
    'La autenticación usa CarRentalDb con EF Core. Los scripts database/01-create-database.sql y database/02-create-auth-tables.sql preparan el esquema y las tablas; la migración InitialAuthSchema permite aplicar la estructura desde Visual Studio.',
    'Los secretos locales se configuran con User Secrets. No se incluyen usuarios, contraseñas, claves ni tokens en este informe.'
])
j.add_heading('Rutas de práctica', level=1)
add_table(j,['Ruta','Comportamiento definido'],[
    ('POST /api/auth/register','Registra una cuenta con rol User y devuelve 201.'),
    ('POST /api/auth/login','Valida credenciales y devuelve un JWT.'),
    ('GET /api/vehiculos','Ruta pública de ejemplo.'),
    ('POST /api/vehiculos','Requiere JWT; en la guía se espera 401 sin token y 201 con token válido.'),
    ('DELETE /api/vehiculos/{id}','Requiere rol Admin; la guía espera 403 para User y 200 para Admin.')
],[2.8,3.8])
j.add_heading('Evidencia observada', level=1)
j.add_paragraph('En la ejecución de la colección local de Postman, Login as local Admin respondió 200 y aprobaron las aserciones de respuesta y presencia de Bearer token. La colección completa registró 14 comprobaciones aprobadas y 0 errores; la captura inferior está recortada para mostrar solo el resultado del login.')
j.add_picture(str(OUT1/'capturas'/'postman-login-jwt.png'), width=Inches(6.1))
caption(j,'Figura 1. Postman: login JWT con HTTP 200 y aserción de token aprobada. El valor del token no se muestra.')
j.save(OUT1/'Informe-Evidencias-Seguridad-JWT.docx')

# Document 2 — ISO countries & customers
c=base('API de países ISO y clientes','Informe de endpoints, procedimientos almacenados y pruebas')
c.add_paragraph('Este informe reúne la evidencia de la segunda práctica. Los controladores de países ISO y clientes responden desde la API local, usan repositorios de CarRentalServices que ejecutan procedimientos almacenados MySQL y cuentan con pruebas unitarias separadas por entidad.')
c.add_heading('Estructura solicitada', level=1)
add_table(c,['Capa','Archivos principales'],[
    ('API CarRental','Controllers/IsoCountriesController.cs y Controllers/CustomersController.cs'),
    ('Contratos','CarRentalServices/Contracts/ICountryRepository.cs e ICustomerRepository.cs'),
    ('Entidades','CarRentalServices/Entities/Country.cs y Customer.cs'),
    ('Repositorios','CarRentalServices/Repositories/CountryRepository.cs y CustomerRepository.cs'),
    ('Pruebas','CarRental.Tests/Countries/CountriesControllerTests.cs y Customers/CustomersControllerTests.cs')
],[1.5,5.1])
c.add_heading('Endpoints y resultado de Postman', level=1)
add_table(c,['Entidad','Método y ruta','Resultado observado'],[
    ('País ISO','GET /IsoCountries','200 OK'),('País ISO','GET /IsoCountries/{id}','200 OK'),('País ISO','POST /IsoCountries','201 Created'),('País ISO','PUT /IsoCountries','204 No Content'),
    ('Cliente','GET /Customers','200 OK'),('Cliente','GET /Customers/{id}','200 OK'),('Cliente','POST /Customers','201 Created'),('Cliente','PUT /Customers','204 No Content')
],[1.0,3.0,2.6])
c.add_paragraph('La última ejecución de la colección de Postman recorrió nueve solicitudes (incluido login) y aprobó 14 aserciones, con 0 errores. La ejecución tomó 1.809 s y promedió 32 ms por respuesta. Las escrituras de la API están protegidas por JWT y rol Admin.')
c.add_heading('Procedimientos almacenados y base de datos', level=1)
c.add_paragraph('El script database/03-create-catalog-stored-procedures.sql define el esquema carrental, las tablas de catálogo y los procedimientos invocados por los repositorios. Para países se usan GetCountries, GetCountryById, InsertCountry y UpdateCountry. Para clientes se usan GetCustomers, GetCustomerById, InsertCustomer, UpdateCustomer y DeleteCustomer. El documento docs/country-customer-stored-procedures.md describe los parámetros y resultados.')
c.add_heading('Captura de respuestas', level=1)
c.add_picture(str(OUT2/'capturas'/'postman-runner-endpoints.png'), width=Inches(6.65))
caption(c,'Figura 1. Postman Runner: 14 comprobaciones aprobadas, 0 errores y respuestas HTTP de los endpoints de países y clientes.')
c.add_heading('Pruebas unitarias', level=1)
c.add_paragraph('Los archivos de prueba están organizados por entidad y cubren los métodos GET, GET por id, POST y PUT. En la revisión anterior de esta solución se informó una ejecución de dotnet test con 22 de 22 pruebas aprobadas. La captura de Postman de este informe valida la integración HTTP; el resultado de pruebas unitarias se informa por separado y no se presenta como una captura de Visual Studio.')
c.add_paragraph('Notas: el runner usa el entorno local CarRental Local. La colección incluye login para obtener el token y pruebas de estado/cuerpo para las rutas de países y clientes. Las respuestas 204 de PUT no tienen cuerpo, como corresponde a los requests capturados.')
c.save(OUT2/'Informe-Evidencias-ISO-Countries-Customers.docx')
print('Created two evidence reports.')


