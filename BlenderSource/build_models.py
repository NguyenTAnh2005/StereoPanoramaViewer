"""
build_scene.py - Tao canh test cho stereo panorama (Blender 4.2, Cycles)

Cach chay: Blender -> tab Scripting -> New -> dan code -> Run Script.
CANH BAO: script XOA moi mesh trong scene (tru vat ten "Plane") roi tao lai.
Hay File > Save As mot ban sao truoc khi chay.

Moi vat the o 1 trong 3 nhom khoang cach:
  near (1.5-2.5 m): do lech stereo lon nhat
  mid  (3-6 m)    : do lech vua
  far  (8-16 m)   : gan nhu khong lech (nen)
"""
import bpy
import math
import random
import colorsys
from mathutils import Vector

# ======================= THAM SO CHINH SUA ==========================
SEED = 7                 # doi so nay de ra bo cuc khac
N_NEAR, N_MID, N_FAR = 3, 5, 5
INTEROCULAR = 0.10       # m (mac dinh Blender 0.065)
CAM_HEIGHT = 1.6         # m
RES_X, RES_Y = 2048, 1024
ENABLE_STEREO = True     # True: render cap L/R. Nho TAT khi render depth.
# Huong "truoc mat" cua panorama (radian). Camera xoay (90,0,0) nhin theo +Y.
FORWARD = math.radians(90)

# (khoang cach tam vat min/max, kich thuoc min/max)
GROUPS = {
    "near": ((1.5, 2.5), (0.4, 0.8)),
    "mid":  ((3.0, 6.0), (0.8, 1.6)),
    "far":  ((8.0, 16.0), (1.5, 3.0)),
}
KINDS = ["cube", "sphere", "ico", "cone", "cylinder"]
# ====================================================================

random.seed(SEED)
scene = bpy.context.scene


def clear_meshes():
    for obj in list(bpy.data.objects):
        if obj.type == "MESH" and obj.name != "Plane":
            bpy.data.objects.remove(obj, do_unlink=True)


def ensure_plane():
    if "Plane" not in bpy.data.objects:
        bpy.ops.mesh.primitive_plane_add(size=100, location=(0, 0, 0))
        bpy.context.active_object.name = "Plane"


def make_material(name, rgb):
    mat = bpy.data.materials.new(name)
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    bsdf.inputs["Base Color"].default_value = (rgb[0], rgb[1], rgb[2], 1.0)
    bsdf.inputs["Roughness"].default_value = 0.5
    return mat


def add_primitive(kind):
    if kind == "cube":
        bpy.ops.mesh.primitive_cube_add(size=1)
    elif kind == "sphere":
        bpy.ops.mesh.primitive_uv_sphere_add(radius=0.5, segments=48, ring_count=24)
        bpy.ops.object.shade_smooth()
    elif kind == "ico":
        bpy.ops.mesh.primitive_ico_sphere_add(radius=0.5, subdivisions=3)
        bpy.ops.object.shade_smooth()
    elif kind == "cone":
        bpy.ops.mesh.primitive_cone_add(radius1=0.5, depth=1, vertices=48)
    else:
        bpy.ops.mesh.primitive_cylinder_add(radius=0.5, depth=1, vertices=48)
    return bpy.context.active_object


def place(kind, size, dist, angle, rgb, name):
    obj = add_primitive(kind)
    tall = random.uniform(1.0, 1.8) if kind in ("cube", "cone", "cylinder") else 1.0
    obj.scale = (size, size, size * tall)
    obj.name = name
    obj.data.materials.append(make_material(name + "_mat", rgb))
    obj.location = (dist * math.cos(angle), dist * math.sin(angle), obj.dimensions.z / 2)
    return obj


def setup_camera():
    cam_obj = scene.camera or bpy.data.objects.get("Camera")
    if cam_obj is None:
        cam_obj = bpy.data.objects.new("Camera", bpy.data.cameras.new("Camera"))
        scene.collection.objects.link(cam_obj)
    scene.camera = cam_obj
    cam_obj.location = (0, 0, CAM_HEIGHT)
    cam_obj.rotation_euler = (math.radians(90), 0, 0)
    cam = cam_obj.data
    cam.type = "PANO"
    try:
        cam.panorama_type = "EQUIRECTANGULAR"
    except AttributeError:
        cam.cycles.panorama_type = "EQUIRECTANGULAR"
    cam.stereo.interocular_distance = INTEROCULAR
    try:
        cam.stereo.convergence_mode = "PARALLEL"
    except Exception:
        pass
    scene.render.engine = "CYCLES"
    scene.render.resolution_x = RES_X
    scene.render.resolution_y = RES_Y
    scene.render.resolution_percentage = 100
    scene.render.use_multiview = ENABLE_STEREO
    scene.render.views_format = "STEREO_3D"


# ----------------------------- BUILD --------------------------------
clear_meshes()
ensure_plane()
setup_camera()

n = N_NEAR + N_MID + N_FAR
step = 2 * math.pi / n

# Vat dau tien (near) nam ngay huong truoc mat, la cho nguoi xem nhin dau tien
rest = ["near"] * (N_NEAR - 1) + ["mid"] * N_MID + ["far"] * N_FAR
random.shuffle(rest)
groups = ["near"] + rest

hues = [i / n for i in range(n)]
random.shuffle(hues)

report = []
for k, grp in enumerate(groups):
    (d_min, d_max), (s_min, s_max) = GROUPS[grp]
    dist = random.uniform(d_min, d_max)
    size = random.uniform(s_min, s_max)
    angle = FORWARD + k * step + random.uniform(-0.25, 0.25) * step
    if k == 0:
        dist, angle = 1.8, FORWARD
    rgb = colorsys.hsv_to_rgb(hues[k], random.uniform(0.7, 1.0), random.uniform(0.7, 1.0))
    kind = random.choice(KINDS)
    obj = place(kind, size, dist, angle, rgb, f"{grp}_{kind}_{k}")
    cam_pos = Vector((0, 0, CAM_HEIGHT))
    report.append((obj.name, (obj.location - cam_pos).length))

print("=== OBJECTS (sorted by distance to camera) ===")
for name, d in sorted(report, key=lambda r: r[1]):
    print(f"{name:24s} dist={d:5.2f} m")
print(f"Stereo 3D: {scene.render.use_multiview} | Interocular: {INTEROCULAR} m")