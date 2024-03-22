CREATE TABLE ta_remote_cetak
(
    fs_kd_trs VARCHAR(20) NOT NULL CONSTRAINT DF_ta_remote_cetak_fs_kd_trs DEFAULT(''),
    fs_jenis_dok VARCHAR(20) NOT NULL CONSTRAINT DF_ta_remote_cetak_fs_jenis_dok DEFAULT(''),
    fd_tgl_send VARCHAR(10) NOT NULL CONSTRAINT DF_ta_remote_cetak_fd_tgl_send DEFAULT(''),
    fs_jam_send VARCHAR(8) NOT NULL CONSTRAINT DF_ta_remote_cetak_fs_jam_send DEFAULT(''),
    fs_remote_addr VARCHAR(100) NOT NULL CONSTRAINT DF_ta_remote_cetak_fs_remote_addr DEFAULT(''),
    fn_cetak DECIMAL(18) NOT NULL CONSTRAINT DF_ta_remote_cetak_fn_cetak DEFAULT(''),
    fd_tgl_cetak VARCHAR(10) NOT NULL CONSTRAINT DF_ta_remote_cetak_fd_tgl_cetak DEFAULT(''),
    fs_jam_cetak VARCHAR(8) NOT NULL CONSTRAINT DF_ta_remote_cetak_fs_jam_cetak DEFAULT(''),
    fs_json_data TEXT NOT NULL CONSTRAINT DF_ta_remote_cetak_fs_json_data DEFAULT(''),
    CallbackDataOfta VARCHAR(255) NOT NULL CONSTRAINT DF_ta_remote_cetak_CallbackDataOfta DEFAULT(''),

    CONSTRAINT PK_ta_remote_cetak PRIMARY KEY CLUSTERED(fs_kd_trs, fs_jenis_dok, fs_remota_addr)  
)