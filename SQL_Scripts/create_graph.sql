use db_POS;

drop table connection;
drop table node;
drop table graph;
go

create table graph(
	id int identity(1,1) primary key,
	bez varchar(10)
);

create table node(
	id int identity(1,1) primary key,
	bez varchar(10), 
	x int, 
	y int,
	graph_id int references graph
);

create table connection(
	node_from int references node,
	node_to int references node,
	dist int,
	Primary Key (node_from, node_to)
);
go