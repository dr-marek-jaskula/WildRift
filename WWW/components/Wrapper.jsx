import { Container } from "@mui/material";

const Wrapper = (props) => {
	const { darkmode } = props;
	return (
		<div className={darkmode? "dark-bg wrapper" : "white-bg wrapper"}> 
			<Container>
				{props.children}
			</Container>
		</div>

	);
};

export default Wrapper;