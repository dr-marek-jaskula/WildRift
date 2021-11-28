const Wrapper = (props) => {
	const { darkmode } = props;
	return (
		<div className={darkmode? "dark-bg" : "white-bg"}>
			{props.children}
		</div>
	);
};

export default Wrapper;